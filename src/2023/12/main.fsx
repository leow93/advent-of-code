#load "../../utils/Utils.fsx"

open System
open System.Collections.Generic
open Utils

let input = Input.readLines ()

type Spring =
  | Operational
  | Damaged
  | Unknown

module Parsing =
  let private parseSpring ch =
    match ch with
    | '.' -> Some Operational
    | '#' -> Some Damaged
    | '?' -> Some Unknown
    | _ -> None

  let private parseInt (x: string) =
    match Int32.TryParse x with
    | true, x -> Some x
    | _ -> None

  let private repeat (n: int) (sep: 'a option) (xs: 'a list) =
    let xs =
      List.init n (fun i ->
        match sep with
        | Some x when i < (n - 1) -> xs @ [ x ]
        | _ -> xs)
      |> List.concat

    xs

  let private parseLine n (line: string) =
    match line.Split [| ' ' |] with
    | [| a; b |] ->
      let springs =
        a.ToCharArray()
        |> Array.choose parseSpring
        |> List.ofArray
        |> repeat n (Some Unknown)

      let sizes = b.Split(',') |> Array.choose parseInt |> List.ofArray |> repeat n None
      (springs, sizes) |> Some
    | _ -> None

  let parseInput duplicateCount (input: string seq) =
    input |> Seq.choose (parseLine duplicateCount) |> List.ofSeq

  let unparseLine (springs, sizes) =
    let springs =
      springs
      |> List.map (function
        | Operational -> '.'
        | Damaged -> '#'
        | Unknown -> '?')
      |> List.toArray
      |> String

    let sizes =
      sizes
      |> List.map (fun x -> x.ToString())
      |> List.map String
      |> List.fold (fun acc s -> acc + s + ",") ""


    String.concat " " [ springs; sizes.Remove(sizes.Length - 1, 1) ]

let skip (n: int) (xs: 'a list) =
  if n > xs.Length then [] else xs |> List.skip n

let cache = Dictionary()

let rec solve (springs, sizes) =
  if cache.ContainsKey(springs, sizes) then
    cache.[(springs, sizes)]
  else
    match springs, sizes with
    | [], [] -> 1L
    | [], _ -> 0L
    | springs, [] when springs |> List.contains Damaged -> 0
    | _, [] -> 1L
    | spring::ss, size::szs ->
      let operationalCount =
        if spring <> Damaged then
          solve (ss, sizes)
        else
          0L

      let damagedCount =
        if spring = Operational then
          0L
        elif size > springs.Length then
          0L
        elif springs |> List.take size |> List.exists (fun x -> x = Operational) then
          0L
        elif springs.Length <> size && springs |> List.tryItem size = Some Damaged then
          0L
        else
          solve (springs |> skip (size + 1), szs)

      let x = operationalCount + damagedCount
      cache.Add((springs, sizes), x)
      x

let partOne input =
  input |> Parsing.parseInput 1 |> List.map solve |> List.sum

let partTwo input =
  input |> Parsing.parseInput 5 |> List.map solve |> List.sum

partOne input |> printfn "Part one: %d"
partTwo input |> printfn "Part two: %A"
