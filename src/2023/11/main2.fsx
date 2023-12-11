#load "../../utils/Utils.fsx"

open Utils
let input = Input.readLines ()

type Cell =
  | Galaxy of int64 * int64
  | Empty of int64 * int64

module Parsing =
  let parse (input: string array) =
    input
    |> Array.mapi (fun i line ->
      line.ToCharArray()
      |> Array.indexed
      |> Array.choose (function
        | j, '#' -> Some(Galaxy(int64 i, int64 j))
        | j, '.' -> Some(Empty(int64 i, int64 j))
        | _ -> None))


let getGalaxies grid =
  let rec inner i acc =
    match grid |> Array.tryItem i with
    | None -> acc
    | Some row ->
      let galaxies =
        row
        |> Array.choose (function
          | Galaxy(x, y) -> Some(x, y)
          | _ -> None)
        |> List.ofArray

      inner (i + 1) (acc @ galaxies)

  inner 0 []

let distance factor empties (a, b) =
  let start = if a < b then a else b
  let finish = if a < b then b else a

  let rec inner x d =
    if x = finish then
      d
    else
      match empties |> List.tryFind ((=) x) with
      | None -> inner (x + 1) (d + 1L)
      | Some _ -> inner (x + 1) (d + int64 factor)

  inner start 0L

let manhattanDistance ((x1, y1), (x2, y2)) = abs (x2 - x1) + abs (y2 - y1)

let isEmpty =
  function
  | Empty _ -> true
  | _ -> false

let getEmptyRows grid =
  grid
  |> Array.indexed
  |> Array.filter (fun (_, row) -> row |> Array.forall isEmpty)
  |> Array.map fst
  |> List.ofArray

let expand factor emptyRows emptyColumns (x, y) =
  let deltaX =
    (emptyRows |> List.filter (fun i -> int64 i < x) |> List.length) * (factor - 1)

  let deltaY =
    (emptyColumns |> List.filter (fun i -> int64 i < y) |> List.length)
    * (factor - 1)

  (x + int64 deltaX, y + int64 deltaY)

let solve factor grid =
  let emptyRows = getEmptyRows grid
  let emptyColumns = getEmptyRows (grid |> Array.transpose)

  let galaxies = getGalaxies grid |> List.map (expand factor emptyRows emptyColumns)

  let pairs =
    galaxies |> List.allPairs galaxies |> List.filter (fun (p1, p2) -> p1 <> p2)

  let half x = x / 2L
  pairs |> List.sumBy manhattanDistance |> half

let grid = Parsing.parse input

solve 2 grid |> printfn "Part 1: %d"
solve 1_000_000 grid |> printfn "Part 2: %d"
