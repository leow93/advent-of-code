#load "../../utils/Utils.fsx"

open System.Collections.Generic
open Utils

let input = Input.readLines ()

type Coord = int * int

type Cell =
  | Empty of Coord
  | NS of Coord
  | EW of Coord
  | NE of Coord
  | NW of Coord
  | SE of Coord
  | SW of Coord

type MarkedCell =
  | LoopPipe of Cell
  | Other of Cell

let item (x, y) grid =
  grid |> Array.tryItem x |> Option.bind (fun row -> row |> Array.tryItem y)

module Parsing =
  let private findStartCoord grid =
    let rec inner i =
      match grid |> Array.tryItem i with
      | None -> None
      | Some row ->
        match row |> Array.tryFindIndex Option.isNone with
        | Some j -> Some(i, j)
        | None -> inner (i + 1)

    inner 0

  let private inferStart start grid =
    let x, y = start

    let above =
      match item (x - 1, y) grid with
      | Some(Some(NS x)) -> Some(NS x)
      | Some(Some(SW x)) -> Some(SW x)
      | Some(Some(SE x)) -> Some(SW x)
      | _ -> None

    let below =
      match item (x + 1, y) grid with
      | Some(Some(NS x)) -> Some(NS x)
      | Some(Some(NW x)) -> Some(NW x)
      | Some(Some(NE x)) -> Some(NE x)
      | _ -> None

    let left =
      match item (x, y - 1) grid with
      | Some(Some(EW x)) -> Some(EW x)
      | Some(Some(NE x)) -> Some(NE x)
      | Some(Some(SE x)) -> Some(SE x)
      | _ -> None

    let right =
      match item (x, y + 1) grid with
      | Some(Some(EW x)) -> Some(EW x)
      | Some(Some(NW x)) -> Some(NW x)
      | Some(Some(SW x)) -> Some(SW x)
      | _ -> None

    match above, below, left, right with
    | Some _, Some _, None, None -> NS start
    | Some _, None, Some _, None -> NW start
    | Some _, None, None, Some _ -> NE start
    | None, Some _, Some _, None -> SW start
    | None, Some _, None, Some _ -> SE start
    | None, None, Some _, Some _ -> EW start
    | _ -> failwith "Could not infer start"

  let parse (lines: string array) =
    let grid =
      lines
      |> Array.mapi (fun i line ->
        line.ToCharArray()
        |> Array.indexed
        |> Array.map (fun (j, c) ->
          let coord = i, j

          match c with
          | '.' -> Empty coord |> Some
          | '|' -> NS coord |> Some
          | '-' -> EW coord |> Some
          | 'L' -> NE coord |> Some
          | 'J' -> NW coord |> Some
          | '7' -> SW coord |> Some
          | 'F' -> SE coord |> Some
          // Keep as None for now, we'll infer the proper start later
          | 'S' -> None
          | _ -> failwithf "Unknown character: %c" c))

    match findStartCoord grid with
    | None -> failwith "No start found"
    | Some start ->
      // here's where we infer the start
      let startCell = inferStart start grid

      let finalGrid =
        grid
        |> Array.map (fun row ->
          row
          |> Array.map (function
            | Some x -> x
            // startCell is the only None
            | None -> startCell))

      finalGrid, startCell

let nextCells coords grid =
  coords |> List.choose (fun coord -> item coord grid)

let getNeighbours grid curr =
  match curr with
  | Empty _ -> []
  | NS(x, y) -> grid |> nextCells [ (x + 1, y); (x - 1, y) ]
  | EW(x, y) -> grid |> nextCells [ (x, y + 1); (x, y - 1) ]
  | NE(x, y) -> grid |> nextCells [ (x, y + 1); (x - 1, y) ]
  | SE(x, y) -> grid |> nextCells [ (x, y + 1); (x + 1, y) ]
  | SW(x, y) -> grid |> nextCells [ (x, y - 1); (x + 1, y) ]
  | NW(x, y) -> grid |> nextCells [ (x, y - 1); (x - 1, y) ]

let walkTheLoop (grid, start) =
  let mutable count = 0
  let queue = Queue<Cell>()
  queue.Enqueue(start)
  let mutable visited: Set<Cell> = Set.ofList [ start ]

  while queue.Count > 0 do
    let curr = queue.Dequeue()
    let neighbours = getNeighbours grid curr

    for n in neighbours do
      if not (visited |> Set.contains n) then
        visited <- visited |> Set.add n
        queue.Enqueue n
        count <- count + 1

  let markedGrid =
    grid
    |> Array.map (fun row ->
      row
      |> Array.map (fun cell ->
        if visited |> Set.contains cell then
          LoopPipe cell
        else
          Other cell))

  System.Math.Ceiling(decimal count / 2M) |> int, markedGrid

let countLine line =
  let rec inner i crossingCount count =
    match line |> Array.tryItem i with
    | None -> count
    | Some(LoopPipe x) ->
      match x with
      | NS _
      | NW _
      | NE _ -> inner (i + 1) (crossingCount + 1) count
      | _ -> inner (i + 1) crossingCount count
    | Some(Other _) when crossingCount % 2 <> 0 -> inner (i + 1) crossingCount (count + 1)
    | Some(Other _) -> inner (i + 1) crossingCount count

  inner 0 0 0

let data = input |> Parsing.parse
let count, markedGrid = walkTheLoop data

count |> printfn "Part one: %i"
markedGrid |> Array.sumBy countLine |> printfn "Part two: %i"
