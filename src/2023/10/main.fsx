#load "../../utils/Utils.fsx"

open System.Collections.Generic
open Utils

let input = Input.readLines ()

type Coord = int * int

type Cell =
  | Empty of Coord
  | Start of Coord
  | NS of Coord
  | EW of Coord
  | NE of Coord
  | NW of Coord
  | SE of Coord
  | SW of Coord

module Parsing =
  let parse (lines: string array) =
    lines
    |> Array.mapi (fun i line ->
      line.ToCharArray()
      |> Array.indexed
      |> Array.choose (fun (j, c) ->
        let coord = i, j

        match c with
        | '.' -> Empty coord |> Some
        | '|' -> NS coord |> Some
        | '-' -> EW coord |> Some
        | 'L' -> NE coord |> Some
        | 'J' -> NW coord |> Some
        | '7' -> SW coord |> Some
        | 'F' -> SE coord |> Some
        | 'S' -> Start coord |> Some
        | _ -> None))

let item (x, y) grid =
  grid |> Array.tryItem x |> Option.bind (fun row -> row |> Array.tryItem y)

let findElement grid element =
  let rec inner i =
    match grid |> Array.tryItem i with
    | None -> None
    | Some row ->
      match row |> Array.tryFindIndex (fun e -> e = element) with
      | Some j -> Some(i, j)
      | None -> inner (i + 1)

  inner 0

let findStart grid =
  let rec inner i =
    match grid |> Array.tryItem i with
    | None -> None
    | Some row ->
      match
        row
        |> Array.tryFindIndex (fun e ->
          match e with
          | Start _ -> true
          | _ -> false)
      with
      | Some j -> Some(i, j)
      | None -> inner (i + 1)

  inner 0

let getStartNeighbours grid curr =
  let x, y = curr

  let above =
    match item (x - 1, y) grid with
    | Some(NS x) -> Some(NS x)
    | Some(SW x) -> Some(SW x)
    | Some(SE x) -> Some(SW x)
    | _ -> None

  let below =
    match item (x + 1, y) grid with
    | Some(NS x) -> Some(NS x)
    | Some(NW x) -> Some(NW x)
    | Some(NE x) -> Some(NE x)
    | _ -> None

  let left =
    match item (x, y - 1) grid with
    | Some(EW x) -> Some(EW x)
    | Some(NE x) -> Some(NE x)
    | Some(SE x) -> Some(SE x)
    | _ -> None

  let right =
    match item (x, y + 1) grid with
    | Some(EW x) -> Some(EW x)
    | Some(NW x) -> Some(NW x)
    | Some(SW x) -> Some(SW x)
    | _ -> None

  [ above; below; left; right ]
  |> List.choose id
  |> (fun x ->
    if x.Length > 2 then
      failwithf "Too many neighbours: %A" x
    else
      x)

let nextCells coords grid =
  coords |> List.choose (fun coord -> item coord grid)

let getNeighbours grid curr =
  match curr with
  | Empty _ -> []
  | Start xy -> getStartNeighbours grid xy
  | NS(x, y) -> grid |> nextCells [ (x + 1, y); (x - 1, y) ]
  | EW(x, y) -> grid |> nextCells [ (x, y + 1); (x, y - 1) ]
  | NE(x, y) -> grid |> nextCells [ (x, y + 1); (x - 1, y) ]
  | SE(x, y) -> grid |> nextCells [ (x, y + 1); (x + 1, y) ]
  | SW(x, y) -> grid |> nextCells [ (x, y - 1); (x + 1, y) ]
  | NW(x, y) -> grid |> nextCells [ (x, y - 1); (x - 1, y) ]

let countLoopSize grid =
  let start = findStart grid

  match start with
  | None -> failwith "No start found"
  | Some start ->
    let mutable count = 0
    let queue = Queue<Cell>()
    queue.Enqueue(Start start)
    let mutable visited: Set<Cell> = Set.ofList [ Start start ]

    while queue.Count > 0 do
      let curr = queue.Dequeue()
      let neighbours = getNeighbours grid curr

      // printfn "curr %A" curr
      // printfn "Neighbours %A" neighbours

      for n in neighbours do
        if not (visited |> Set.contains n) then
          visited <- visited |> Set.add n
          queue.Enqueue n
          count <- count + 1


    System.Math.Ceiling(decimal count / 2M)

let partOne input =
  let grid = input |> Parsing.parse
  grid |> countLoopSize


partOne input |> printfn "Part one: %A"
