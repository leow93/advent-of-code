#load "../../utils/Utils.fsx"

open System.Collections.Generic
open Utils

let input = Input.readLines ()

type Pulse =
  | High
  | Low

type From = string
type To = string
type Message = From * Pulse * To list

type IHandlePulse =
  abstract member handlePulse: From * Pulse -> Message option
  abstract member Id: string
  abstract member Outputs: string list
  abstract member addOutput: string -> unit
  abstract member addInput: string -> unit

module Broadcaster =
  type Broadcaster(id: string, outputs) =
    let mutable state = outputs
    member this.handlePulse = (this :> IHandlePulse).handlePulse

    interface IHandlePulse with
      member this.Id = id
      member this.Outputs = state

      member this.addInput _ = ()

      member this.addOutput(output: string) = state <- List.append state [ output ]

      member this.handlePulse(_from: string, pulse: Pulse) = Some(id, pulse, state)

module FlipFlop =
  type InternalState =
    | On
    | Off

  type State =
    { internalState: InternalState
      outputs: string list }

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

      member _.handlePulse(from: string, pulse: Pulse) =
        let internalState, messages =
          match pulse, state.internalState with
          | High, state -> state, None
          | Low, On -> Off, Some(id, Low, state.outputs)
          | Low, Off -> On, Some(id, High, state.outputs)

        state <-
          { state with
              internalState = internalState }

        messages


module Conjunction =
  type State =
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
          // not sure about this? other way around?
          Some(id, Low, state.outputs)
        else
          state <- nextState
          Some(id, High, state.outputs)


type Module =
  | Broadcaster of IHandlePulse
  | FlipFlop of IHandlePulse
  | Conjunction of IHandlePulse

type Modules = Map<string, Module>

module Parsing =
  let private parseInput (string: string) : Module =
    match string with
    | "broadcaster" -> Broadcaster(Broadcaster.Broadcaster("broadcaster", []))
    | s when s.StartsWith("%") -> FlipFlop(FlipFlop.FlipFlop(s.Substring(1)))
    | s when s.StartsWith("&") -> Conjunction(Conjunction.Conjunction(s.Substring(1)))
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
          | Broadcaster x ->
            for output in outputs do
              x.addOutput output

            acc @ [ x.Id, Broadcaster x ]
          | FlipFlop x ->
            for output in outputs do
              x.addOutput output

            acc @ [ x.Id, FlipFlop x ]
          | Conjunction x ->
            for output in outputs do
              x.addOutput output

            acc @ [ x.Id, Conjunction x ]

        )
        []

    // add inputs to conjunctions
    let conjunctionIds =
      modules
      |> List.filter (function
        | _, Conjunction _ -> true
        | _ -> false)
      |> List.map fst

    modules
    |> List.iter (fun (id, m) ->
      match m with
      | Module.Broadcaster x ->
        let conjunctionOutputs =
          x.Outputs |> List.filter (fun x -> conjunctionIds |> List.contains x)

        conjunctionOutputs
        |> List.iter (fun conjunctionId ->
          match modules |> List.tryFind (fun (id, _) -> id = conjunctionId) with
          | Some(_, Conjunction con) -> con.addInput id
          | _ -> ())

      | Module.FlipFlop x ->
        let conjunctionOutputs =
          x.Outputs |> List.filter (fun x -> conjunctionIds |> List.contains x)

        conjunctionOutputs
        |> List.iter (fun conjunctionId ->
          match modules |> List.tryFind (fun (id, _) -> id = conjunctionId) with
          | Some(_, Conjunction con) -> con.addInput id
          | _ -> ())
      | _ -> ())

    modules

let modules = input |> Parsing.parse

let pushButton modules =
  let mutable highPulses = 0
  let mutable lowPulses = 1 // button sends low pulse to broadcaster
  let queue = Queue<Message>()

  let enqueue message =
    queue.Enqueue message

    match message with
    | _, High, _ -> highPulses <- highPulses + 1
    | _, Low, _ -> lowPulses <- lowPulses + 1

  let rec loop () =
    match queue.Count with
    | 0 -> ()
    | _ ->
      let from, pulse, outputs = queue.Dequeue()

      outputs
      |> List.map (fun output ->
        match modules |> List.tryFind (fun (id, _) -> id = output) with
        | Some(_, Broadcaster b) -> b.handlePulse (from, pulse)
        | Some(_, FlipFlop f) -> f.handlePulse (from, pulse)
        | Some(_, Conjunction c) -> c.handlePulse (from, pulse)
        | _ -> None)
      |> List.choose id
      |> List.iter (fun (from, pulse, outputs) ->
        for output in outputs do
          enqueue (from, pulse, [ output ]))

      loop ()

  match modules |> List.tryFind (fun x -> fst x = "broadcaster") with
  | Some(_, Broadcaster broadcaster) ->
    match broadcaster.handlePulse ("button", Low) with
    | None -> failwith "Couldn't broadcast message"
    | Some(from, pulse, outputs) ->
      for output in outputs do
        enqueue (from, pulse, [ output ])
  | _ -> failwith "Couldn't find broadcaster"

  loop ()
  lowPulses, highPulses


let partOne () =
  let initialLowPulses, initialHighPulses = 0, 0

  { 1..1000 }
  |> Seq.fold
    (fun (modules, lo, hi) _ ->
      let lowPulses, highPulses = pushButton modules
      modules, lo + lowPulses, hi + highPulses)
    (modules, initialLowPulses, initialHighPulses)
  |> fun (_, lowPulses, highPulses) -> lowPulses * highPulses

partOne () |> printfn "part one: %d"
