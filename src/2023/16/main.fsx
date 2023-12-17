#load "../../utils/Utils.fsx"

open Utils

let input = Input.readLines ()

type Cell =
  | Empty
  | MirrorForward
  | MirrorBackward
  | HorizontalSplitter
  | VerticalSplitter

let parse (lines: string[]) =
  lines
  |> Array.map (fun line ->
    line.ToCharArray()
    |> Array.choose (function
      | '.' -> Some Empty
      | '/' -> Some MirrorForward
      | '\\' -> Some MirrorBackward
      | '-' -> Some HorizontalSplitter
      | '|' -> Some VerticalSplitter
      | _ -> None))

type Direction =
  | Up
  | Down
  | Left
  | Right

type Position =
  { direction: Direction
    coords: int * int
    cell: Cell }

let getNextCells position grid =
  let x, y = position.coords

  match position.direction, position.cell with
  | Right, Empty
  | Up, MirrorForward
  | Down, MirrorBackward
  | Right, HorizontalSplitter ->
    grid
    |> Array.tryItem x
    |> Option.bind (Array.tryItem (y + 1))
    |> Option.map (fun cell ->
      [ { cell = cell
          coords = (x, y + 1)
          direction = Right } ])
    |> Option.defaultValue []
  | Left, Empty
  | Down, MirrorForward
  | Up, MirrorBackward
  | Left, HorizontalSplitter ->
    grid
    |> Array.tryItem x
    |> Option.bind (Array.tryItem (y - 1))
    |> Option.map (fun cell ->
      [ { cell = cell
          coords = (x, y - 1)
          direction = Left } ])
    |> Option.defaultValue []
  | Up, Empty
  | Right, MirrorForward
  | Left, MirrorBackward
  | Up, VerticalSplitter ->
    grid
    |> Array.tryItem (x - 1)
    |> Option.bind (Array.tryItem y)
    |> Option.map (fun cell ->
      [ { cell = cell
          coords = (x - 1, y)
          direction = Up } ])
    |> Option.defaultValue []
  | Down, Empty
  | Right, MirrorBackward
  | Left, MirrorForward
  | Down, VerticalSplitter ->
    grid
    |> Array.tryItem (x + 1)
    |> Option.bind (Array.tryItem y)
    |> Option.map (fun cell ->
      [ { cell = cell
          coords = (x + 1, y)
          direction = Down } ])
    |> Option.defaultValue []
  | Left, VerticalSplitter
  | Right, VerticalSplitter ->
    [ (Down, (x + 1, y)); (Up, (x - 1, y)) ]
    |> List.choose (fun (direction, (x, y)) ->
      grid
      |> Array.tryItem x
      |> Option.bind (Array.tryItem y)
      |> Option.map (fun cell ->
        { cell = cell
          coords = (x, y)
          direction = direction }))
  | Up, HorizontalSplitter
  | Down, HorizontalSplitter ->
    [ Right, (x, y + 1); Left, (x, y - 1) ]
    |> List.choose (fun (direction, (x, y)) ->
      grid
      |> Array.tryItem x
      |> Option.bind (Array.tryItem y)
      |> Option.map (fun cell ->
        { cell = cell
          coords = (x, y)
          direction = direction }))

let rec guideBeam curr grid visited visitedSplits =

  match getNextCells curr grid with
  | [] -> visited |> Set.add curr.coords, visitedSplits
  | [ x ] -> guideBeam x grid (visited |> Set.add curr.coords) visitedSplits

  | [ a; b ] ->
    let visited = visited |> Set.add curr.coords

    if visitedSplits |> List.contains (a, b) then
      visited, visitedSplits
    else
      let visitedSplits' = List.append visitedSplits [ a, b ]
      let visited, splits = guideBeam a grid (visited |> Set.add a.coords) visitedSplits'
      guideBeam b grid (visited |> Set.add b.coords) splits
  | _ -> failwith "More than two cells"

let countEnergizedCells grid initial =
  let visited, _ =
    guideBeam initial grid ([ initial.coords ] |> Set.ofList) List.empty

  visited |> Set.count


let getStartingPositions (grid: Cell[][]) =
  let rowCount = grid.Length
  let colCount = grid.[0].Length

  seq {
    for i = 0 to (rowCount - 1) do
      for j = 0 to (colCount - 1) do
        if i = 0 then
          yield
            { coords = (i, j)
              direction = Down
              cell = grid[i][j] }

        if j = 0 then
          yield
            { coords = (i, j)
              direction = Right
              cell = grid[i][j] }

        if j = (colCount - 1) then
          yield
            { coords = (i, j)
              direction = Left
              cell = grid[i][j] }

        if i = (rowCount - 1) then
          yield
            { coords = (i, j)
              direction = Up
              cell = grid[i][j] }
  }

let partOne input =
  let grid = input |> parse
  let firstCell = grid[0][0]

  let initial =
    { direction = Right
      coords = (0, 0)
      cell = firstCell }

  countEnergizedCells grid initial

input |> partOne |> printfn "\n\nPart 1: %d"

let partTwo input =
  let grid = input |> parse
  let startingPositions = getStartingPositions grid
  startingPositions |> Seq.map (countEnergizedCells grid) |> Seq.max

input |> partTwo |> printfn "Part 2: %d"
