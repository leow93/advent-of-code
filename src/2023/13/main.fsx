#load "../../utils/Utils.fsx"

open Utils
let input = Input.readText ()

type Cell =
  | Ash
  | Rock

module Parsing =
  let private parseCell =
    function
    | '.' -> Some Ash
    | '#' -> Some Rock
    | _ -> None

  let private parseBlock (text: string) =
    text.Split("\n")
    |> Array.map (fun row -> row.ToCharArray() |> Array.choose parseCell)

  let parse (text: string) =
    text.Split("\n\n") |> Array.map parseBlock

module Mirrors =
  type Mode =
    | H
    | V

  type Mirror =
    | Horizontal of int
    | Vertical of int

  let private checkIsMirror block i =
    if i = (block |> Array.length) - 1 then
      false
    else
      let rec inner block i isAMirror =
        if not isAMirror then
          false
        else
          match block |> Array.tryItem i, block |> Array.tryItem (i + 1) with
          | Some curr, Some next ->
            if curr <> next then
              false
            else
              let nextBlock = block |> Array.removeManyAt i 2
              inner nextBlock (i - 1) true
          | Some _, None -> isAMirror
          | None, Some _ -> isAMirror
          | None, None -> isAMirror

      inner block i true

  let findMirrors block =
    let mutable mirrors = []

    for i in 0 .. ((block |> Array.length) - 1) do
      if checkIsMirror block i then
        mirrors <- mirrors @ [ Horizontal i ]

    let transposed = block |> Array.transpose

    for i in 0 .. ((transposed |> Array.length) - 1) do
      if checkIsMirror transposed i then
        mirrors <- mirrors @ [ Vertical i ]

    mirrors

  let private updateBlockAt i j value block =
    block
    |> Array.mapi (fun x row -> row |> Array.mapi (fun y cell -> if x = i && y = j then value else cell))

  let alternativeBlocks block =
    let coords =
      seq {
        for i in 0 .. ((block |> Array.length) - 1) do
          let row = block[0]

          for j in 0 .. ((row |> Array.length) - 1) do
            yield (i, j)
      }

    coords
    |> Seq.choose (fun (x, y) ->
      match block |> Array.tryItem x with
      | None -> None
      | Some row ->
        match row |> Array.tryItem y with
        | None -> None
        | Some cell when cell = Ash -> Some(updateBlockAt x y Rock block)
        | Some _ -> Some(updateBlockAt x y Ash block))
    |> Array.ofSeq

let partOne input =
  input
  |> Parsing.parse
  |> Array.map Mirrors.findMirrors
  |> Array.sumBy (fun mirrors ->
    match mirrors |> List.tryHead with
    | None -> 0
    | Some(Mirrors.Horizontal x) -> (100 * (x + 1))
    | Some(Mirrors.Vertical x) -> x + 1)

let partTwo input =
  let blocks = input |> Parsing.parse

  let originalMirrors =
    blocks
    |> Array.indexed
    |> Array.map (fun (i, b) -> i, Mirrors.findMirrors b)
    |> Array.choose (fun (i, m) ->
      match m |> List.tryHead with
      | None -> None
      | Some x -> Some(i, x))
    |> Map.ofArray

  let lookupOriginal i = originalMirrors |> Map.find i

  let mutable result = 0

  for i in 0 .. ((blocks |> Array.length) - 1) do
    let block = blocks[i]
    let alternatives = Mirrors.alternativeBlocks block
    let original = lookupOriginal i

    let alternativeMirror =
      alternatives
      |> Array.map Mirrors.findMirrors
      |> Array.map (List.filter (fun x -> x <> original))
      |> Array.tryFind (fun xs -> xs.Length > 0)
      |> Option.map List.head

    match alternativeMirror, original with
    | None, Mirrors.Horizontal x
    | Some(Mirrors.Horizontal x), _ -> result <- result + (100 * (x + 1))
    | None, Mirrors.Vertical x
    | Some(Mirrors.Vertical x), _ -> result <- result + (x + 1)

  result

input |> partOne |> printfn "Part 1: %d"
input |> partTwo |> printfn "Part 2: %d"
