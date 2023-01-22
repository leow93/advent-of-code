let readLines = System.IO.File.ReadAllLines

type Cell =
  | On
  | Off

type Grid = Cell [] []

module Grid =
  let parse grid : Grid =
    grid
    |> Array.map (fun line ->
      line
      |> Array.choose (function
        | '#' -> Some On
        | '.' -> Some Off
        | _ -> None))

  let private countLiveNeighbours (grid: Grid) (x, y) =
    [ grid
      |> Array.tryItem (x + 1)
      |> Option.bind (Array.tryItem y)
      grid
      |> Array.tryItem (x - 1)
      |> Option.bind (Array.tryItem y)
      grid
      |> Array.tryItem x
      |> Option.bind (Array.tryItem (y + 1))
      grid
      |> Array.tryItem x
      |> Option.bind (Array.tryItem (y - 1))
      grid
      |> Array.tryItem (x + 1)
      |> Option.bind (Array.tryItem (y + 1))
      grid
      |> Array.tryItem (x + 1)
      |> Option.bind (Array.tryItem (y - 1))
      grid
      |> Array.tryItem (x - 1)
      |> Option.bind (Array.tryItem (y + 1))
      grid
      |> Array.tryItem (x - 1)
      |> Option.bind (Array.tryItem (y - 1)) ]
    |> List.filter (fun x -> x = Some On)
    |> List.length

  let private updateCell cell liveNeighbours =
    match cell with
    | On when liveNeighbours = 2 || liveNeighbours = 3 -> On
    | On -> Off
    | Off when liveNeighbours = 3 -> On
    | Off -> Off

  let updatePartOne grid : Grid =
    grid
    |> Array.mapi (fun x row ->
      row
      |> Array.mapi (fun y cell -> updateCell cell (countLiveNeighbours grid (x, y))))

  let getCorners grid =
    let maxX, maxY =
      ((grid |> Array.length) - 1, (grid |> Array.item 0 |> Array.length) - 1)

    [ (0, 0)
      (maxX, 0)
      (0, maxY)
      (maxX, maxY) ]

  let isCorner corners coord = corners |> List.contains coord

  let updatePartTwo grid : Grid =
    let corners = getCorners grid

    grid
    |> Array.mapi (fun x row ->
      row
      |> Array.mapi (fun y cell ->
        match isCorner corners (x, y) with
        | true -> On
        | false -> updateCell cell (countLiveNeighbours grid (x, y))))

let parseFile file =
  file
  |> readLines
  |> Array.map (fun line -> line.ToCharArray())
  |> Grid.parse

let main grid fn =
  { 1..100 }
  |> Seq.fold (fun grid _ -> fn grid) grid
  |> Array.sumBy (fun row ->
    row
    |> Array.sumBy (function
      | On -> 1
      | Off -> 0))

let grid = parseFile "./data.txt"
let partOne = main grid Grid.updatePartOne
partOne |> printfn "Part I: %i"

let corners = Grid.getCorners grid

let grid' =
  grid
  |> Array.mapi (fun x row ->
    row
    |> Array.mapi (fun y cell ->
      match Grid.isCorner corners (x, y) with
      | true -> On
      | false -> cell))

let partTwo = main grid' Grid.updatePartTwo // 815 too low

partTwo |> printfn "Part II: %i"
