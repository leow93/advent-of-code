#load "../../utils/Utils.fsx"

open Utils

let input = Input.readLines ()

type Grid = Cell list list
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

  let parse (lines: string array) =
    lines
    |> List.ofArray
    |> List.map (fun line -> line.ToCharArray() |> List.ofArray |> parseCells)

type Message = { count: int; coord: int * int }

let findStart grid =
  let rec inner i =
    match grid |> List.tryItem i with
    | None -> None
    | Some row ->
      match row |> List.tryFindIndex (fun c -> c = Start) with
      | Some j -> Some(i, j)
      | None -> inner (i + 1)

  inner 0

let nextSteps (i, j) grid =
  let rows = grid |> List.length
  let cols = grid[0] |> List.length

  [ (i + 1, j); (i - 1, j); (i, j + 1); (i, j - 1) ]
  |> List.choose (fun (x, y) ->
    if x >= 0 && x < rows && y >= 0 && y < cols && grid[x][y] <> Rock then
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

let rec partOne input =
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
  
input |> partOne |> printfn "Part one %d"
