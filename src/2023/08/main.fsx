#load "../../utils/Utils.fsx"

open Utils

type Instruction =
  | Left
  | Right

module Parsing =
  let private parseInstructions (line: string) =
    line
    |> Strings.toCharArray
    |> Array.choose (function
      | 'L' -> Some Left
      | 'R' -> Some Right
      | _ -> None)

  let private buildMap map line =
    match line |> Strings.split " = " with
    | [| node; leftRight |] ->
      match leftRight[1 .. leftRight.Length - 2] |> Strings.split ", " with
      | [| left; right |] -> map |> Map.add node (left, right)
      | _ -> map
    | _ -> map

  let parse (lines: string array) =
    let instructions = lines |> Array.take 1 |> Array.head |> parseInstructions
    let map = lines |> Array.skip 2 |> Array.fold buildMap Map.empty
    instructions, map

let getInstruction (instructions: Instruction array) index =
  instructions |> Array.tryItem (index % instructions.Length)

let walkNode (instructions, map) finished (start: string) =
  let rec inner i curr =
    if finished curr then
      i
    else
      match map |> Map.tryFind curr with
      | Some(left, right) ->
        match getInstruction instructions i with
        | Some Left -> inner (i + 1) left
        | Some Right -> inner (i + 1) right
        | None -> i
      | None -> i

  inner 0 start

let partOne input =
  let finished (x: string) = x = "ZZZ"
  let data = input |> Parsing.parse
  walkNode data finished "AAA"

module Maths =
  let rec private greatestCommonDivisor x y =
    match x with
    | 0L -> y
    | _ when y = 0L -> x
    | _ -> greatestCommonDivisor y (x % y)

  let private lowestCommonMultiple x y = x * y / (greatestCommonDivisor x y)

  let rec lcm xs =
    match xs with
    | [ x; y ] -> lowestCommonMultiple x y
    | x :: xs -> lcm [ x; (lcm xs) ]
    | [] -> failwith "Impossible"

let partTwo input =
  let nodesEndingWith (x: char) (map: Map<string, _>) =
    map |> Map.filter (fun node _ -> node.EndsWith x) |> Map.keys |> List.ofSeq

  let data = input |> Parsing.parse
  let nodesEndingWithA = nodesEndingWith 'A' (snd data)
  let finished (x: string) = x.EndsWith('Z')

  nodesEndingWithA
  |> List.map (walkNode data finished)
  |> List.map int64
  |> Maths.lcm

let input = Input.readLines ()
partOne input |> printfn "Part 1: %d"
partTwo input |> printfn "Part 2: %A"
