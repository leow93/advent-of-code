#load "../../utils/Utils.fsx"

open System.Collections.Generic
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
    match state.grid.TryGetValue((i, j)) with
    | true, cell -> cell
    | false, _ ->
      let normalisedI = i % state.rows |> fun x -> if x < 0 then state.rows + x else x

      let normalisedJ = j % state.cols |> fun x -> if x < 0 then state.cols + x else x

      state.grid[normalisedI, normalisedJ]

  let nextSteps (i, j) grid =
    [ (i + 1, j); (i - 1, j); (i, j + 1); (i, j - 1) ]
    |> List.choose (fun (x, y) -> if grid |> get (x, y) <> Rock then Some(x, y) else None)


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

let nextSteps (i, j) grid =
  let rows = grid.rows
  let cols = grid.cols

  [ (i + 1, j); (i - 1, j); (i, j + 1); (i, j - 1) ]
  |> List.choose (fun (x, y) ->
    if x >= 0 && x < rows && y >= 0 && y < cols && grid.grid[x, y] <> Rock then
      Some(x, y)
    else
      None)

let printGrid grid visited =
  let rows = grid |> List.length
  let cols = grid[0] |> List.length

  for i = 0 to (rows - 1) do
    for j = 0 to (cols - 1) do
      if grid[i][j] = Start then
        printf "S"
      elif visited |> List.exists (fun c -> c = (i, j)) then
        printf "O"
      elif grid[i][j] = Rock then
        printf "#"
      else
        printf "."

    printfn ""

  printfn "\n"

let partOne input =
  let grid = Parsing.parse input

  let rec getNextSteps grid acc =
    match acc |> Set.count with
    | 0 -> getNextSteps grid (grid |> findStart |> Option.toList |> Set.ofList)
    | _ ->
      acc
      |> Set.fold
        (fun acc coord ->
          let next = nextSteps coord grid
          Set.union acc (next |> Set.ofList))
        Set.empty

  let rec loop remainingSteps acc =
    match remainingSteps with
    | 0 -> acc
    | n -> loop (n - 1) (getNextSteps grid acc)

  loop 64 Set.empty |> Set.count

let partTwo input =
  let grid = Parsing.parse input

  let rec getNextSteps grid acc =
    match acc |> Set.count with
    | 0 -> getNextSteps grid (grid |> findStart |> Option.toList |> Set.ofList)
    | _ ->
      acc
      |> Set.fold
        (fun acc coord ->
          let next = InfiniteGrid.nextSteps coord grid
          Set.union acc (next |> Set.ofList))
        Set.empty

  let rec loop remainingSteps acc =
    match remainingSteps with
    | 0 -> acc
    | n -> loop (n - 1) (getNextSteps grid acc)

  let numberOfIterations = 500
  loop numberOfIterations Set.empty |> Set.count

input |> partOne |> printfn "Part one %d"
input |> partTwo |> printfn "Part two: %d"
