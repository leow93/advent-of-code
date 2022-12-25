open System
open System.Collections.Generic

type Coord = int * int

type Cell =
  | Rock
  | Air
  | Sand

let matrix a b =
  let mutable result = Set.empty

  for x in a do
    for y in b do
      result <- result |> Set.add (x, y)

  result

let maxBy fn coords =
  coords
  |> List.fold
       (fun x row ->
         let rowMax = row |> List.map fn |> List.max
         if rowMax > x then rowMax else x)
       0

let minBy fn coords =
  coords
  |> List.fold
       (fun x row ->
         let rowMin = row |> List.map fn |> List.min
         if rowMin < x then rowMin else x)
       Int32.MaxValue


let buildSeq start finish =
  match start <= finish with
  | true -> [ start..finish ]
  | _ -> [ finish..start ]

type Grid = Dictionary<Coord, Cell>

module Grid =
  let private setValues (grid: Grid) (start: Coord) (finish: Coord) (value: Cell) =
    let startX, startY = start
    let finishX, finishY = finish

    let xSeq = buildSeq startX finishX
    let ySeq = buildSeq startY finishY

    matrix xSeq ySeq
    |> Set.iter (fun key -> grid[key] <- value)

    grid

  let rec private drawLine (value: Cell) (grid: Grid) (line: Coord list) =
    match line with
    | [ x; y ] -> setValues grid x y value
    | [ _ ] -> grid
    | head :: tail ->
      let grid' =
        setValues grid head (tail |> List.head) value

      drawLine value grid' tail
    | _ -> grid

  let private join (s: string) (xs: string array) = System.String.Join(s, xs)

  let init (lines: Coord list list) =
    let dict = Dictionary()
    let minX = minBy fst lines
    let maxX = maxBy fst lines
    let maxY = maxBy snd lines

    for i in [ minX..maxX ] do
      for j in [ 0..maxY ] do
        dict[(i, j)] <- Air

    lines |> List.fold (drawLine Rock) dict


let split (sep: string) (s: string) = s.Split sep |> List.ofArray

let parseLine line =
  line
  |> split " -> "
  |> List.map (split ",")
  |> List.choose (function
    | [ a; b ] -> Coord(int a, int b) |> Some
    | _ -> None)

let parseFileToCoords file =
  System.IO.File.ReadAllLines file
  |> List.ofArray
  |> (List.map parseLine)

let sandEntry = Coord(500, 0)

module SandUnit =
  type NextPosition =
    | Position of Coord
    | Void

  let isAir (grid: Grid) coord =
    match grid.TryGetValue(coord) with
    | true, x -> x = Air
    | false, _ -> true

  let getNextPosition (grid: Grid) position minX minY maxX maxY =
    let x, y = position
    if y + 1 > maxY || x <= minX || x >= maxX then
      Some Void
    else
      let below = (x, y + 1)
      let belowLeft = (x - 1, y + 1)
      let belowRight = (x + 1, y + 1)

      if isAir grid below then
        Some(Position below)
      elif isAir grid belowLeft then
        Some(Position belowLeft)
      elif isAir grid belowRight then
        Some(Position belowRight)
      else
        None

  let move (grid: Grid) minX minY maxX maxY =
    let rec inner position =
      match getNextPosition grid position minX minY maxX maxY with
      | None -> Some position
      | Some Void -> None
      | Some (Position next) -> inner next

    if grid[sandEntry] = Sand then
      false
    else
      match inner sandEntry with
      | Some coord ->
        grid[coord] <- Sand
        true
      | None -> false

let getDimensions coords =
  let maxX = maxBy fst coords
  let maxY = maxBy snd coords
  let minX = minBy fst coords
  let minY = minBy snd coords
  maxX, maxY, minX, minY

let partOne file =
  let coords = parseFileToCoords file

  let maxX, maxY, minX, minY =
    getDimensions coords

  let grid = coords |> Grid.init

  let mutable count = 0

  while SandUnit.move grid minX minY maxX maxY do
    count <- count + 1

  count

let partTwo file =
  let coords = parseFileToCoords file
  let maxX, maxY, _, _ = getDimensions coords

  let extraCoords =
    [ [ Coord(0, maxY + 2)
        Coord(maxX * 2, maxY + 2) ] ]

  let coords = List.append coords extraCoords

  let maxX, maxY, minX, minY =
    getDimensions coords

  let grid = coords |> Grid.init

  let mutable count = 0

  while SandUnit.move grid minX minY maxX maxY do
    count <- count + 1

  count


partOne "./test.txt"
|> printfn "Part I (test): %i"

partOne "./data.txt"
|> printfn "Part I (real): %i"

partTwo "./test.txt"
|> printfn "Part II (test): %i"

partTwo "./data.txt"
|> printfn "Part II (real): %i"
