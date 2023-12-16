#load "../../utils/Utils.fsx"

open System.Web
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

  let private nextIndex (rocks: Cell[][]) i j =
    match rocks[i][j] with
    | RoundRock ->
      let mutable result = i

      while result > 0 && rocks[result - 1][j] = Empty do
        result <- result - 1

      Some result
    | _ -> None

  let private slideRocksNorth rocks =
    let mutable result = rocks

    for i in 0 .. Array.length rocks - 1 do
      let row = result[i]

      for j in 0 .. Array.length row - 1 do
        match nextIndex result i j with
        | None -> ()
        | Some x ->
          result[i][j] <- Empty
          result[x][j] <- RoundRock

    result

  let tilt direction rocks =
    match direction with
    | N -> rocks |> slideRocksNorth
    | _ -> rocks

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


let partOne input =
  input |> Parsing.parseInput |> (Tilting.tilt Tilting.N) |> platformLoad


input |> partOne |> printfn "Part one: %d"
