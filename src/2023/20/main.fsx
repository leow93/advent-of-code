#load "../../utils/Utils.fsx"

open System.Collections.Generic
open Utils

let input = Input.readLines ()

type Pulse =
  | High
  | Low

type From = string
type To = string
type Message = From * Pulse * To
let toMessage from pulse to_ = from, pulse, to_

type IHandlePulse =
  abstract member handlePulse: From * Pulse -> Message list
  abstract member Id: string
  abstract member Outputs: string list
  abstract member addOutput: string -> unit
  abstract member addInput: string -> unit

type Broadcaster(id: string, outputs) =
  let mutable state = outputs
  member this.handlePulse = (this :> IHandlePulse).handlePulse

  interface IHandlePulse with
    member this.Id = id
    member this.Outputs = state

    member this.addInput _ = ()

    member this.addOutput(output: string) = state <- List.append state [ output ]

    member this.handlePulse(_from: string, pulse: Pulse) = state |> List.map (toMessage id pulse)

type FlipFlopState =
  { internalState: InternalState
    outputs: string list }

and InternalState =
  | On
  | Off

type FlipFlop(id: string) =
  let mutable state = { internalState = Off; outputs = [] }

  interface IHandlePulse with
    member _.Id = id

    member this.Outputs = state.outputs
    member this.addInput _ = ()

    member _.addOutput(output: string) =
      state <-
        { state with
            outputs = List.append state.outputs [ output ] }

    member _.handlePulse(_from: string, pulse: Pulse) =
      let internalState, messages =
        match pulse, state.internalState with
        | High, state -> state, []
        | Low, On -> Off, state.outputs |> List.map (toMessage id Low)
        | Low, Off -> On, state.outputs |> List.map (toMessage id High)

      state <-
        { state with
            internalState = internalState }

      messages

type ConjunctionState =
  {
    // Remembers the last pulse received for each input module
    inputs: Map<string, Pulse>
    outputs: string list }


type Conjunction(id: string) =
  let mutable state = { inputs = Map.empty; outputs = [] }

  interface IHandlePulse with
    member _.addInput(input: string) =
      state <-
        { state with
            inputs = Map.add input Low state.inputs }

    member _.addOutput(output: string) =
      state <-
        { state with
            outputs = List.append state.outputs [ output ] }

    member _.Id = id
    member _.Outputs = state.outputs

    member _.handlePulse(from: string, pulse: Pulse) =
      let nextState =
        { state with
            inputs = Map.add from pulse state.inputs }

      let allHigh = nextState.inputs |> Map.forall (fun _ pulse -> pulse = High)

      if allHigh then
        state <- nextState
        state.outputs |> List.map (toMessage id Low)
      else
        state <- nextState
        state.outputs |> List.map (toMessage id High)


type Module =
  | B of IHandlePulse
  | FF of IHandlePulse
  | C of IHandlePulse

type Modules = Map<string, Module>

module Parsing =
  let private parseInput (string: string) : Module =
    match string with
    | "broadcaster" -> B(Broadcaster("broadcaster", []))
    | s when s.StartsWith("%") -> FF(FlipFlop(s.Substring(1)))
    | s when s.StartsWith("&") -> C(Conjunction(s.Substring(1)))
    | s -> failwithf "Couldn't parse input: %s" s

  let parseOutputs (string: string) =
    string.Split(", ") |> Array.map (fun s -> s.Trim()) |> Array.toList

  let parse (lines: string[]) =
    let x =
      lines
      |> Array.choose (fun line ->
        match line.Split(" -> ") with
        | [| input; outputs |] -> Some(parseInput input, parseOutputs outputs)
        | _ -> None)

    let modules =
      x
      |> Array.fold
        (fun acc (input, outputs) ->
          match input with
          | B x ->
            for output in outputs do
              x.addOutput output

            acc @ [ x.Id, B x ]
          | FF x ->
            for output in outputs do
              x.addOutput output

            acc @ [ x.Id, FF x ]
          | C x ->
            for output in outputs do
              x.addOutput output

            acc @ [ x.Id, C x ]

        )
        []

    // add inputs to conjunctions
    let conjunctionIds =
      modules
      |> List.filter (function
        | _, C _ -> true
        | _ -> false)
      |> List.map fst

    modules
    |> List.iter (fun (id, m) ->
      match m with
      | Module.B x ->
        let conjunctionOutputs =
          x.Outputs |> List.filter (fun x -> conjunctionIds |> List.contains x)

        conjunctionOutputs
        |> List.iter (fun conjunctionId ->
          match modules |> List.tryFind (fun (id, _) -> id = conjunctionId) with
          | Some(_, C con) -> con.addInput id
          | _ -> ())

      | Module.FF x ->
        let conjunctionOutputs =
          x.Outputs |> List.filter (fun x -> conjunctionIds |> List.contains x)

        conjunctionOutputs
        |> List.iter (fun conjunctionId ->
          match modules |> List.tryFind (fun (id, _) -> id = conjunctionId) with
          | Some(_, C con) -> con.addInput id
          | _ -> ())
      | _ -> ())

    modules


let pushButton modules cb =
  let mutable highPulses = 0
  let mutable lowPulses = 1 // button sends low pulse to broadcaster
  let queue = Queue<Message>()

  let mutable reachedDest = false

  let enqueue message =
    let from, pulse, _= message

    match cb with
    | Some f when pulse = High && f from -> reachedDest <- true
    | _ -> ()

    queue.Enqueue message

    match message with
    | _, High, _ -> highPulses <- highPulses + 1
    | _, Low, _ -> lowPulses <- lowPulses + 1


  let rec loop () =
    if reachedDest then
      ()
    else
      match queue.Count with
      | 0 -> ()
      | _ ->
        let from, pulse, dest = queue.Dequeue()

        match modules |> List.tryFind (fun (id, _) -> id = dest) with
        | Some(_, B b) -> b.handlePulse (from, pulse)
        | Some(_, FF f) -> f.handlePulse (from, pulse)
        | Some(_, C c) -> c.handlePulse (from, pulse)
        | _ -> []
        |> List.iter enqueue

        loop ()

  match modules |> List.tryFind (fun x -> fst x = "broadcaster") with
  | Some(_, B broadcaster) ->
    match broadcaster.handlePulse ("button", Low) with
    | [] -> failwith "Couldn't broadcast message"
    | messages ->
      for msg in messages do
        enqueue msg
  | _ -> failwith "Couldn't find broadcaster"

  loop ()
  lowPulses, highPulses, reachedDest


let partOne () =
  let modules = input |> Parsing.parse
  let initialLowPulses, initialHighPulses = 0, 0

  { 1..1000 }
  |> Seq.fold
    (fun (modules, lo, hi) _ ->
      let lowPulses, highPulses, _ = pushButton modules None
      modules, lo + lowPulses, hi + highPulses)
    (modules, initialLowPulses, initialHighPulses)
  |> fun (_, lowPulses, highPulses) -> lowPulses * highPulses

let getPressesForModuleToSendHigh modules id =
  let mutable reachedDest = false
  let mutable buttonPresses = 0

  while not reachedDest do
    let _, _, reached = pushButton modules (Some(fun x -> x = id))
    buttonPresses <- buttonPresses + 1

    if reached then
      reachedDest <- true

  buttonPresses

let partTwo () =
   // vg, nb, vc and ls must all send a high pulse for rx to have a low pulse
  [ "vg"; "nb"; "vc"; "ls" ]
  |> List.map (fun x -> getPressesForModuleToSendHigh (input |> Parsing.parse) x)
  |> List.map int64
  |> Maths.lcm

partOne () |> printfn "part one: %d"
partTwo () |> printfn "part two: %A"
