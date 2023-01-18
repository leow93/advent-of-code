open System
open System.Collections.Generic

let readLines = System.IO.File.ReadAllLines

let split (sep: string) (s: string) = s.Split sep |> List.ofArray

type Wire = string

type Arg =
  | Constant of uint16
  | Wire of string

type Instruction =
  | Number of Arg
  | And of (Arg * Arg)
  | Or of (Arg * Arg)
  | LShift of (Arg * Arg)
  | RShift of (Arg * Arg)
  | Not of Arg

type Command = Instruction * Wire

module Instruction =
  let private parseArg (a: string) =
    if Char.IsLetter(a[0]) then
      Wire a
    else
      Constant(uint16 a)

  let parse (s: string) =
    match split " -> " s with
    | [ instruction; outputWire ] when instruction.Contains " AND " ->
      match split " AND " instruction with
      | [ a; b ] -> (And(parseArg a, parseArg b), outputWire) |> Some
      | _ -> None
    | [ instruction; outputWire ] when instruction.Contains " OR " ->
      match split " OR " instruction with
      | [ a; b ] -> (Or(parseArg a, parseArg b), outputWire) |> Some
      | _ -> None
    | [ instruction; outputWire ] when instruction.Contains " LSHIFT " ->
      match split " LSHIFT " instruction with
      | [ a; b ] ->
        (LShift(parseArg a, parseArg b), outputWire)
        |> Some
      | _ -> None
    | [ instruction; outputWire ] when instruction.Contains " RSHIFT " ->
      match split " RSHIFT " instruction with
      | [ a; b ] ->
        (RShift(parseArg a, parseArg b), outputWire)
        |> Some
      | _ -> None
    | [ instruction; outputWire ] when instruction.StartsWith "NOT " ->
      match split "NOT " instruction with
      | [ ""; a ] -> (Not(parseArg a), outputWire) |> Some
      | _ -> None
    | [ instruction; outputWire ] ->
      match Int32.TryParse instruction with
      | true, x -> (Number(Constant(uint16 x)), outputWire) |> Some
      | false, _ -> (Number(Wire instruction), outputWire) |> Some
    | _ -> None

let evalInstruction (state: Dictionary<Wire, uint16>) (action, target) =

  let canEvalArg a =
    match a with
    | Constant _ -> true
    | Wire w -> state.ContainsKey w

  let evalArg a =
    match a with
    | Constant n -> n
    | Wire w -> state[w]

  let canEvalAction =
    match action with
    | Number a
    | Not a -> (canEvalArg a)
    | And (a, b)
    | Or (a, b)
    | LShift (a, b)
    | RShift (a, b) -> (canEvalArg a) && (canEvalArg b)

  if canEvalAction then
    match action with
    | Number a -> evalArg a
    | Not a -> ~~~(evalArg a)
    | And (a, b) -> (evalArg a) &&& (evalArg b)
    | Or (a, b) -> (evalArg a) ||| (evalArg b)
    | LShift (a, b) -> (evalArg a) <<< int (evalArg b)
    | RShift (a, b) -> (evalArg a) >>> int (evalArg b)
    |> Some
  else
    None

let partOne instructions =
  let circuit = Dictionary<_, _>()
  let queue = Queue<Instruction * Wire>()
  instructions |> Seq.iter queue.Enqueue

  while queue.Count > 0 do
    let instruction = queue.Dequeue()

    match evalInstruction circuit instruction with
    | Some x -> circuit[snd instruction] <- x
    | None -> queue.Enqueue instruction

  circuit

let instructions =
  readLines "./data.txt"
  |> Array.choose Instruction.parse

let partOneResult = partOne instructions

printfn "Part I: %A" (partOneResult["a"])

let partTwo instructions =
  let instructions' =
    instructions
    |> Seq.map (fun (instruction, wire) ->
      if wire = "b" then
        (Number(Constant(partOneResult["a"])), "b")
      else
        (instruction, wire))

  partOne instructions'

partTwo instructions
|> (fun x -> x["a"])
|> printfn "Part II: %A"
