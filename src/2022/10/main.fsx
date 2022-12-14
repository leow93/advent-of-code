let readLines = System.IO.File.ReadAllLines

type Instruction =
  | Noop
  | Add of int

module Instruction =
  let parse s =
    if s = "noop" then
      [| Noop |]
    elif s.StartsWith "addx" then
      [| Noop; Add(s.Substring 5 |> int) |]
    else
      failwithf "unknown instruction: %s" s

module State =
  type State = { count: int; history: int list }
  let initial = { count = 1; history = [ 1 ] }

  let private addToHistory history x = List.append history [ x ]

  let private addCurrentCountToHistory state = addToHistory state.history state.count

  let evolve state instruction =
    match instruction with
    | Noop -> { state with history = addCurrentCountToHistory state }
    | Add x ->
      let count = state.count + x

      { count = count
        history = addToHistory state.history count }

let relevantCycles (i, _) = i = 20 || (i - 20) % 40 = 0
let signalStrength (a, b) = a * b

let partOne file =
  readLines file
  |> Array.map Instruction.parse
  |> Array.concat
  |> Array.fold State.evolve State.initial
  |> (fun state ->
    state.history
    |> List.indexed
    |> List.map (fun (i, x) -> (i + 1, x))
    |> List.take 221
    |> List.filter relevantCycles
    |> List.sumBy signalStrength)

let partTwo file =
  readLines file
  |> Array.map Instruction.parse
  |> Array.concat
  |> Array.fold State.evolve State.initial
  |> (fun state ->
    state.history
    |> List.mapi (fun i spriteIdx ->
      let position = i % 40

      match abs (spriteIdx - position) < 2 with
      | true -> '#'
      | false -> '.'))
  |> List.iteri (fun i char ->
    if i % 40 = 0 then printf "\n"
    printf "%c" char)


partOne "./test.txt"
|> printfn "Part I (TEST): %i"

partOne "./data.txt"
|> printfn "Part I (REAL): %i"

partTwo "./test.txt"
partTwo "./data.txt"
