#load "../../utils/Utils.fsx"

open System
open System.Collections.Generic
open Utils

let input = Input.readLines ()

let grid =
  input
  |> Array.map (fun s -> s.ToCharArray() |> Array.map (fun c -> int c - int '0'))

let goal = (Array.length grid - 1, Array.length grid[0] - 1)

type Direction =
  | Up
  | Down
  | Left
  | Right

let opposite =
  function
  | Up -> Down
  | Down -> Up
  | Left -> Right
  | Right -> Left

let moveForward dir rows cols (row, col) =
  match dir with
  | Up when row > 0 -> Some(row - 1, col)
  | Down when row < (rows - 1) -> Some(row + 1, col)
  | Left when col > 0 -> Some(row, col - 1)
  | Right when col < (cols - 1) -> Some(row, col + 1)
  | _ -> None


type Crucible =
  { cost: int
    pos: int * int
    dir: Direction
    movesInDir: int }

let item (x, y) = grid[x][y]

let getNextNodesPartOne crucible grid =
  let rows = Array.length grid
  let cols = Array.length grid[0]
  let directions = [ Up; Down; Left; Right ]

  let rec inner i acc =
    match directions |> List.tryItem i with
    | None -> acc
    | Some dir when dir = crucible.dir && crucible.movesInDir = 3 -> inner (i + 1) acc
    | Some dir when dir = opposite crucible.dir -> inner (i + 1) acc
    | Some dir ->
      match moveForward dir rows cols crucible.pos with
      | None -> inner (i + 1) acc
      | Some pos ->
        let cost = crucible.cost + item pos
        let movesInDir = if crucible.dir = dir then crucible.movesInDir + 1 else 1

        let newAcc =
          List.append
            acc
            [ { cost = cost
                pos = pos
                dir = dir
                movesInDir = movesInDir } ]

        inner (i + 1) newAcc

  inner 0 []

let getNextNodesPartTwo crucible grid =
  let rows = Array.length grid
  let cols = Array.length grid[0]
  let directions = [ Up; Down; Left; Right ]

  let rec inner i acc =
    match directions |> List.tryItem i with
    | None -> acc
    // if we've moved 10 times in the same direction, we can't move that direction anymore
    | Some dir when dir = crucible.dir && crucible.movesInDir = 10 -> inner (i + 1) acc
    // must move at least 4 times in same direction
    | Some dir when dir <> crucible.dir && crucible.movesInDir < 4 -> inner (i + 1) acc
    // can't go backwards
    | Some dir when dir = opposite crucible.dir -> inner (i + 1) acc
    | Some dir ->
      match moveForward dir rows cols crucible.pos with
      | None -> inner (i + 1) acc
      | Some pos ->
        let cost = crucible.cost + item pos
        let movesInDir = if crucible.dir = dir then crucible.movesInDir + 1 else 1

        let newAcc =
          List.append
            acc
            [ { cost = cost
                pos = pos
                dir = dir
                movesInDir = movesInDir } ]

        inner (i + 1) newAcc

  inner 0 []

let solve getNextNodes =
  let q = PriorityQueue()
  let seen = HashSet()

  let enqueue crucible =
    // lower cost should go to the front of the queue
    q.Enqueue(crucible, crucible.cost)

  let right =
    { cost = item (0, 1)
      pos = (0, 1)
      dir = Right
      movesInDir = 1 }

  enqueue right

  let down =
    { cost = item (1, 0)
      pos = (1, 0)
      dir = Down
      movesInDir = 1 }

  enqueue down

  let mutable result = Int32.MaxValue

  while q.Count > 0 do
    let crucible = q.Dequeue()

    if crucible.pos = goal && crucible.cost < result then
      result <- crucible.cost
    else
      for node in getNextNodes crucible grid do
        if seen.Add(node.pos, node.dir, node.movesInDir) then
          enqueue node

  result

let partOne = solve getNextNodesPartOne
let partTwo = solve getNextNodesPartTwo

partOne |> printfn "Part 1: %d"
partTwo |> printfn "Part 2: %d"
