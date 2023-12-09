#load "../../utils/Utils.fsx"

open Utils

module Parsing =
  let parse (lines: string array) =
    lines
    |> Array.choose (fun line ->
      match line.Split(' ') with
      | [||] -> None
      | xs -> xs |> Array.map int |> Seq.ofArray |> Some)

let diffs = Seq.pairwise >> Seq.map (fun (a, b) -> b - a)

let auxiliarySeqs (sequence: int seq) =
  let rec inner (acc: int seq list) =
    match acc |> List.tryHead with
    | None -> acc
    | Some x when x |> Seq.forall ((=) 0) -> acc
    | Some x -> inner (diffs x :: acc)

  inner [ sequence ]

let extrapolateForwards (sequences: int seq list) =
  let rec inner i acc =
    match acc |> List.tryItem i with
    | None -> acc
    | Some xs when xs |> Seq.forall ((=) 0) -> inner (i + 1) (acc |> List.updateAt i (Seq.append xs [ 0 ]))
    | Some xs ->
      match acc |> List.tryItem (i - 1) with
      | None -> acc
      | Some ys ->
        match xs |> Seq.tryLast, ys |> Seq.tryLast with
        | Some x, Some y -> inner (i + 1) (acc |> List.updateAt i (Seq.append xs [ x + y ]))
        | _ -> acc

  (inner 0 sequences) |> List.last |> Seq.last

let extrapolateBackwards (sequences: int seq list) =
  let rec inner i acc =
    match acc |> List.tryItem i with
    | None -> acc
    | Some xs when xs |> Seq.forall ((=) 0) -> inner (i + 1) (acc |> List.updateAt i (Seq.append [ 0 ] xs))
    | Some xs ->
      match acc |> List.tryItem (i - 1) with
      | None -> acc
      | Some ys ->
        match xs |> Seq.tryHead, ys |> Seq.tryHead with
        | Some x, Some y -> inner (i + 1) (acc |> List.updateAt i (Seq.append [ x - y ] xs))
        | _ -> acc

  (inner 0 sequences) |> List.last |> Seq.head

let input = Input.readLines ()

let partOne input =
  let data = input |> Parsing.parse
  data |> Array.sumBy (fun s -> auxiliarySeqs s |> extrapolateForwards)

let partTwo input =
  let data = input |> Parsing.parse
  data |> Array.sumBy (fun s -> auxiliarySeqs s |> extrapolateBackwards)

partOne input |> printfn "Part 1: %A"
partTwo input |> printfn "Part 2: %A"
