#load "../../utils/Utils.fsx"

open System.Collections.Generic
open System.Numerics
open Utils

let input = Input.readLines ()

type Grid =
  { rows: int
    cols: int
    grid: Dictionary<int * int, Cell> }

and Cell =
  | Start
  | Garden
  | Rock

module Parsing =
  let private parseCells chs =
    let rec inner xs acc =
      match xs with
      | x :: xs ->
        match x with
        | 'S' -> inner xs (acc @ [ Start ])
        | '#' -> inner xs (acc @ [ Rock ])
        | '.' -> inner xs (acc @ [ Garden ])
        | _ -> failwith "invalid input"
      | [] -> acc

    inner chs []

  let private ofListGrid grid =
    let result = Dictionary<int * int, Cell>()
    let rows = grid |> List.length
    let cols = grid[0] |> List.length

    for i = 0 to (rows - 1) do
      for j = 0 to (cols - 1) do
        result.Add((i, j), grid[i][j])

    { rows = rows
      cols = cols
      grid = result }


  let parse (lines: string array) =
    lines
    |> List.ofArray
    |> List.map (fun line -> line.ToCharArray() |> List.ofArray |> parseCells)
    |> ofListGrid

module InfiniteGrid =
  let get (i, j) (state: Grid) =
    let i = i % state.rows |> fun x -> if x < 0 then state.rows + x else x
    let j = j % state.cols |> fun x -> if x < 0 then state.cols + x else x
    state.grid[i, j]

  let nextSteps (i, j) grid =
    [ (i + 1, j); (i - 1, j); (i, j + 1); (i, j - 1) ]
    |> List.filter (fun (x, y) -> grid |> get (x, y) <> Rock)

type Message = { count: int; coord: int * int }

let findStart (grid: Grid) =
  let keys = grid.grid.Keys |> List.ofSeq

  let rec inner i =
    match keys |> List.tryItem i with
    | Some k ->
      match grid.grid[k] with
      | Start -> Some k
      | _ -> inner (i + 1)
    | None -> None

  inner 0

let singleGridNextSteps (i, j) grid =
  let rows = grid.rows
  let cols = grid.cols

  [ (i + 1, j); (i - 1, j); (i, j + 1); (i, j - 1) ]
  |> List.choose (fun (x, y) ->
    if x >= 0 && x < rows && y >= 0 && y < cols && grid.grid[x, y] <> Rock then
      Some(x, y)
    else
      None)

let rec getNextSteps neighboursFn grid acc =
  match acc |> Set.count with
  | 0 -> getNextSteps neighboursFn grid (grid |> findStart |> Option.toList |> Set.ofList)
  | _ ->
    acc
    |> Set.fold
      (fun acc coord ->
        let next = neighboursFn coord grid
        Set.union acc (next |> Set.ofList))
      Set.empty

let partOne input =
  let grid = Parsing.parse input

  let rec loop remainingSteps acc =
    match remainingSteps with
    | 0 -> acc
    | n -> loop (n - 1) (getNextSteps singleGridNextSteps grid acc)

  loop 64 Set.empty |> Set.count

let partTwo input =
  let grid = Parsing.parse input

  let rec loop remainingSteps acc =
    match remainingSteps with
    | 0 -> acc
    | n -> loop (n - 1) (getNextSteps InfiniteGrid.nextSteps grid acc)

  (*
  Let f(n) be the number of spaces you can reach after n steps.
  Let X be the length of your input grid. 
  f(n), f(n+X), f(n+2X), ...., is a quadratic, so you can find it by finding the first 3 values, then use that to interpolate the final answer.
  *)

  // let size = grid.rows
  // let f0 = loop (size / 2) Set.empty |> Set.count
  // let f1 = loop (size + size / 2) Set.empty |> Set.count
  // let f2 = loop (2 * size + (size / 2)) Set.empty |> Set.count
  // printfn "f0: %d, f1: %d, f2: %d" f0 f1 f2
  // https://www.wolframalpha.com/input?i=quadratic+fit+calculator&assumption=%7B%22F%22%2C+%22QuadraticFitCalculator%22%2C+%22data3x%22%7D+-%3E%22%7B0%2C+1%2C+2%7D%22&assumption=%7B%22F%22%2C+%22QuadraticFitCalculator%22%2C+%22data3y%22%7D+-%3E%22%7B3703%2C+32712%2C+90559%7D%22
  let a = BigInteger(14419)
  let b = BigInteger(14590L)
  let c = BigInteger(3703L)
  let f (x: bigint) = (a * x * x) + (b * x) + c
  ((26501365 - 65) / 131) |> BigInteger |> f

input |> partOne |> printfn "Part one %i"
input |> partTwo |> printfn "Part two: %A"
