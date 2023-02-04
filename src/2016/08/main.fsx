open System

type Cell =
  | On
  | Off

type Grid = Cell array array

let initialGrid =
  Array.create 6 (Array.create 50 Off)

type Command =
  | Rect of (int * int)
  | RotateColumn of (int * int)
  | RotateRow of (int * int)

module Input =
  let parseLine (line: string) =
    if line.StartsWith "rect " then
      match line.Substring(5).Split("x") with
      | [| a; b |] -> Rect(int a, int b) |> Some
      | _ -> None
    elif line.StartsWith "rotate row " then
      match line
              .Substring(11)
              .Split([| "y="; " by " |], StringSplitOptions.RemoveEmptyEntries)
        with
      | [| a; b |] -> RotateRow(int a, int b) |> Some
      | _ -> None
    elif line.StartsWith "rotate column " then
      match line
              .Substring(14)
              .Split([| "x="; " by " |], StringSplitOptions.RemoveEmptyEntries)
        with
      | [| a; b |] -> RotateColumn(int a, int b) |> Some
      | _ -> None
    else
      None

// # # . .
// . # # .
// . . # #
// # . . #
let findReplacementCell row shift i =
  let amount = shift % (row |> Array.length)
  let d = i - amount

  let replacementIdx =
    if d < 0 then
      (row |> Array.length) - amount + i
    else
      d

  row |> Array.item replacementIdx

let evolve state command =
  match command with
  | Rect (x, y) ->
    state
    |> Array.mapi (fun j row ->
      row
      |> Array.mapi (fun i cell -> if i < x && j < y then On else cell))
  | RotateRow (y, amount) ->
    state
    |> Array.mapi (fun j row ->
      row
      |> Array.mapi (fun i cell ->
        if j = y then
          findReplacementCell row amount i
        else
          cell))
  | RotateColumn (x, amount) ->
    state
    |> Array.transpose
    |> Array.mapi (fun i column ->
      column
      |> Array.mapi (fun j cell ->
        if i = x then
          findReplacementCell column amount j
        else
          cell))
    |> Array.transpose

let join (s: string) (xs: string array) = String.Join(s, xs)

let printGrid (grid) =
  grid
  |> Array.map (
    Array.map (function
      | On -> "#"
      | Off -> " ")
    >> join ""
  )
  |> join "\n"

let input =
  System.IO.File.ReadAllLines "./input.txt"

let commands =
  input |> Array.choose Input.parseLine

let grid =
  commands
  |> Array.fold evolve initialGrid

let partOne =
  grid
  |> Array.sumBy (
    Array.sumBy (function
      | On -> 1
      | Off -> 0)
  )
printfn "Part I: %i" partOne

printfn "Part II"
grid |> printGrid |> printfn "%s"