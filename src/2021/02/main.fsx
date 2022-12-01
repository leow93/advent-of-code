let readLines = System.IO.File.ReadLines

module Instruction =
  type Direction =
    | Forwards of int
    | Up of int
    | Down of int

  let parse (s: string) =
    match s.Split " " with
    | [| direction; quantity |] ->
      match direction with
      | "forward" -> Forwards(int quantity)
      | "up" -> Up(int quantity)
      | "down" -> Down(int quantity)
      | _ -> failwith "Unknown direction"
    | _ -> failwith "Unknown instruction"

module PartOne =
  type State = int * int
  let initialState = 0, 0

  let evolve (x, y) direction =
    match direction with
    | Instruction.Forwards v -> (x + v, y)
    | Instruction.Up v -> (x, y - v)
    | Instruction.Down v -> (x, y + v)

let multiply (a, b) = a * b

printfn "Test"

readLines "./test.txt"
|> Seq.map Instruction.parse
|> Seq.fold PartOne.evolve PartOne.initialState
|> multiply
|> printf "%i\n"

readLines "./data.txt"
|> Seq.map Instruction.parse
|> Seq.fold PartOne.evolve PartOne.initialState
|> multiply
|> printf "%i\n"

// runPart2 "./test.txt"

printfn "Actual"
// runPart1 "./data.txt"
// runPart2 "./data.txt"
