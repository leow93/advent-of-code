#load "../../utils/Utils.fsx"

open Utils
let input = Input.readLines ()

type Cell =
  | Galaxy of int * int
  | Empty of int * int

module Parsing =
  let parse (input: string array) =
    input
    |> Array.mapi (fun i line ->
      line.ToCharArray()
      |> Array.indexed
      |> Array.choose (function
        | j, '#' -> Some(Galaxy(i, j))
        | j, '.' -> Some(Empty(i, j))
        | _ -> None))

let grid = Parsing.parse input

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

let manhattanDistance factor emptyRows emptyCols (x1, y1) (x2, y2) =
  abs (distance factor emptyRows (x1, x2))
  + abs (distance factor emptyCols (y1, y2))

let isEmpty =
  function
  | Empty _ -> true
  | _ -> false

let solve factor (grid: Cell array array) =
  let emptyRows =
    grid
    |> Array.indexed
    |> Array.filter (fun (_, row) -> row |> Array.forall isEmpty)
    |> Array.map fst
    |> List.ofArray

  let emptyCols =
    grid
    |> Array.transpose
    |> Array.indexed
    |> Array.filter (fun (_, col) -> col |> Array.forall isEmpty)
    |> Array.map fst
    |> List.ofArray

  let galaxies = getGalaxies grid

  let pairs =
    galaxies |> List.allPairs galaxies |> List.filter (fun (p1, p2) -> p1 <> p2)

  let distances =
    [ for p1, p2 in pairs do
        let distance = manhattanDistance factor emptyRows emptyCols p1 p2
        yield distance ]

  distances |> List.sum |> (fun x -> x / 2L)

let partOne = solve 2
let partTwo = solve 1_000_000 

partOne grid |> printfn "Part 1: %d"
partTwo grid |> printfn "Part 2: %d"
