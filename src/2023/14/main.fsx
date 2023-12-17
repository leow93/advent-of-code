#load "../../utils/Utils.fsx"

open Utils

let input = Input.readLines ()

type Cell =
  | RoundRock
  | SquareRock
  | Empty

module Parsing =

  let private parseLine (str: string) =
    str
    |> Seq.map (function
      | '.' -> Empty
      | '#' -> SquareRock
      | 'O' -> RoundRock
      | _ -> failwith "invalid input")
    |> Seq.toArray

  let parseInput (input: string array) = input |> Array.map parseLine

module Tilting =
  type Direction =
    | N
    | S
    | E
    | W

  let private slideRocksNorth rocks =
    for i in 0 .. Array.length rocks - 1 do
      let row = rocks[i]

      for j in 0 .. Array.length row - 1 do
        match rocks[i][j] with
        | RoundRock ->
          let mutable result = i

          while result > 0 && rocks[result - 1][j] = Empty do
            result <- result - 1

          rocks[i][j] <- Empty
          rocks[result][j] <- RoundRock
        | _ -> ()

    rocks

  let private slideRocksSouth rocks =
    for i = ((Array.length rocks) - 1) downto 0 do
      for j = 0 to ((Array.length rocks[0]) - 1) do
        match rocks[i][j] with
        | RoundRock ->
          let mutable result = i

          while result < rocks.Length - 1 && rocks[result + 1][j] = Empty do
            result <- result + 1

          rocks[i][j] <- Empty
          rocks[result][j] <- RoundRock
        | _ -> ()

    rocks

  let private slideRocksEast rocks =
    for i = 0 to Array.length rocks - 1 do
      for j = (Array.length rocks[0] - 1) downto 0 do
        match rocks[i][j] with
        | RoundRock ->
          let mutable result = j

          while result < rocks[0].Length - 1 && rocks[i][result + 1] = Empty do
            result <- result + 1

          rocks[i][j] <- Empty
          rocks[i][result] <- RoundRock
        | _ -> ()

    rocks

  let private slideRocksWest rocks =
    for i = 0 to Array.length rocks - 1 do
      for j = 0 to Array.length rocks[0] - 1 do
        match rocks[i][j] with
        | RoundRock ->
          let mutable result = j

          while result > 0 && rocks[i][result - 1] = Empty do
            result <- result - 1

          rocks[i][j] <- Empty
          rocks[i][result] <- RoundRock
        | _ -> ()

    rocks

  let tilt direction rocks =
    match direction with
    | N -> rocks |> slideRocksNorth
    | S -> rocks |> slideRocksSouth
    | E -> rocks |> slideRocksEast
    | W -> rocks |> slideRocksWest

  let toString grid =
    let foldRow row =
      (row
       |> Array.fold
         (fun r row ->
           match row with
           | Empty -> r + "."
           | RoundRock -> r + "O"
           | SquareRock -> r + "#")
         "")

    grid |> Array.fold (fun acc row -> acc + foldRow row + "\n") ""

  let cycle = tilt N >> tilt W >> tilt S >> tilt E

let platformLoad grid =
  let length = Array.length grid

  grid
  |> Array.indexed
  |> Array.sumBy (fun (i, row) ->
    let n = length - i

    let roundCount =
      row
      |> Array.sumBy (function
        | RoundRock -> 1
        | _ -> 0)

    n * roundCount)


open Tilting

let partOne input =
  input |> Parsing.parseInput |> (tilt N) |> platformLoad

let partTwo input =
  let limit = 1_000_000_000

  let rec helper idx history grid =

    if idx >= limit then
      grid
    else
      let grid' = grid |> cycle

      let content = toString grid'

      match history |> List.tryFindIndex ((=) content) with
      | None -> helper (idx + 1) (content :: history) grid'
      | Some index ->
        let cycle = index + 1
        let q, _ = System.Math.DivRem(limit - idx, cycle)
        let x = idx + q * cycle + 1
        helper x (content :: history) grid'

  let grid = input |> Parsing.parseInput

  helper 0 [ grid |> toString ] grid |> platformLoad

input |> partOne |> printfn "Part one: %d"
input |> partTwo |> printfn "Part two: %d"
