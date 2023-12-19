#load "../../utils/Utils.fsx"

open System
open Utils

let input = Input.readLines ()

type Instruction =
  | Up of int64
  | Down of int64
  | Left of int64
  | Right of int64

type DigPlan = Instruction list

module Parsing =
  let private parseDig (direction: string) (count: string) =
    match direction with
    | "U" -> Up(int64 count)
    | "D" -> Down(int64 count)
    | "L" -> Left(int64 count)
    | "R" -> Right(int64 count)
    | _ -> failwith "Unknown direction"

  let parsePartOne (input: string[]) =
    input
    |> List.ofArray
    |> List.choose (fun line ->

      match line |> Strings.split " " with
      | [| direction; count; _ |] -> Some(parseDig direction count)
      | _ -> None)

  let parsePartTwo (input: string[]) =
    input
    |> List.ofArray
    |> List.choose (fun line ->
      match line |> Strings.split " " with
      | [| _; _; colour |] ->
        let colour = colour.Replace("#", "").Replace("(", "").Replace(")", "")
        let distanceHex = colour.Substring(0, 5)
        let directionHex = colour.Substring(5, 1)
        let distance = Convert.ToInt64(distanceHex, 16)

        match directionHex with
        | "0" -> Some(Right distance)
        | "1" -> Some(Down distance)
        | "2" -> Some(Left distance)
        | "3" -> Some(Up distance)
        | _ -> None
      | _ -> None)


module Digger =
  let private evolve sites instruction =
    match sites |> List.tryLast, instruction with
    | None, _ -> sites
    | Some(x, y), Up amount -> List.append sites [ (x, y + amount) ]
    | Some(x, y), Down amount -> List.append sites [ (x, y - amount) ]
    | Some(x, y), Right amount -> List.append sites [ (x + amount, y) ]
    | Some(x, y), Left amount -> List.append sites [ (x - amount), y ]

  let getDigSites instructions =
    instructions |> List.fold evolve [ (0, 0) ]

let getArea (coords: (int64 * int64) list) =
  // calculate the determinants of each matrix.
  // Matrix is given by the coordinates of the two points, as a pair.

  let rec loop i sum =
    if i = coords.Length then
      sum
    else
      let x1, y1 = coords[i]
      let x2, y2 = coords[(i + 1) % coords.Length]
      loop (i + 1) (sum + ((x1 * y2) - (x2 * y1)))

  loop 0 0L |> abs |> (fun x -> x / 2L)

let solve parse input =
  let instructions = parse input

  let perimeter =
    instructions
    |> List.sumBy (function
      | Up y
      | Down y
      | Left y
      | Right y -> y)

  let digSites = instructions |> Digger.getDigSites
  let enclosedArea = digSites |> getArea
  1L + (perimeter / 2L) + enclosedArea

let partOne = solve Parsing.parsePartOne
let partTwo = solve Parsing.parsePartTwo

partOne input |> printfn "Part one: %i"
partTwo input |> printfn "Part two: %i"
