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

let forward dir rows cols (row, col) =
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

let children crucible grid =
  let rows = Array.length grid
  let cols = Array.length grid[0]
  let directions = [ Up; Down; Left; Right ]

  let rec inner i acc =
    match directions |> List.tryItem i with
    | None -> acc
    | Some dir when dir = crucible.dir && crucible.movesInDir = 3 -> inner (i + 1) acc
    | Some dir when dir = opposite crucible.dir -> inner (i + 1) acc
    | Some dir ->
      match forward dir rows cols crucible.pos with
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

let partOne () =
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
      for child in children crucible grid do
        if seen.Add(child.pos, child.dir, child.movesInDir) then
          enqueue child

  result

partOne () |> printfn "Part 1: %d"
