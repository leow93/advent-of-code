let readLines = System.IO.File.ReadAllLines

type Grid = int [] []

let toArray (s: string) = s.ToCharArray() |> Array.map string

let isTallest (height: int) (row: int []) =
  row
  |> Array.fold (fun v h -> v && height > h) true

let split x array =
  let a, b = array |> Array.splitAt x
  (a, b |> Array.tail)

let isVisible (grid: Grid) (height, i, j) =
  let tallest = isTallest height

  let beforeX, afterX = grid[i] |> (split j)

  let beforeY, afterY =
    Array.get (grid |> Array.transpose) j |> split i

  tallest beforeX
  || tallest afterX
  || tallest beforeY
  || tallest afterY

let getScore height (row: int []) =
  match row |> Array.tryFindIndex (fun x -> x >= height) with
  | None -> row.Length
  | Some idx -> idx + 1

let scenicScore (grid: Grid) (height, i, j) =
  let score = getScore height
  let beforeX, afterX = grid[i] |> (split j)

  let beforeY, afterY =
    Array.get (grid |> Array.transpose) j |> split i

  score (beforeX |> Array.rev)
  * score afterX
  * score (beforeY |> Array.rev)
  * score afterY

let partOne file =
  let grid =
    readLines file
    |> Array.map toArray
    |> Array.map (fun line -> line |> Array.map int)

  grid
  |> Array.mapi (fun i row ->
    row
    |> Array.mapi (fun j height -> isVisible grid (height, i, j)))
  |> Array.sumBy (fun x -> x |> Array.filter id |> Array.length)

let partTwo file =
  let grid =
    readLines file
    |> Array.map toArray
    |> Array.map (fun line -> line |> Array.map int)

  grid
  |> Array.mapi (fun i row ->
    row
    |> Array.mapi (fun j height -> scenicScore grid (height, i, j)))
  |> Array.map(Array.max) |> Array.max

partOne "./test.txt"
|> printfn "Test part one: %i"

partTwo "./test.txt"
|> printfn "Test part two: %A"

partOne "./data.txt"
|> printfn "Actual part one: %i"
partTwo "./data.txt"
|> printfn "Actual part two: %i"