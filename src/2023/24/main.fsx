#load "../../utils/Utils.fsx"

open System
open Utils

let input = Input.readLines ()

type Position = { x: float; y: float; z: float }

type Velocity = { x: float; y: float; z: float }

type Hailstone =
  { position: Position
    velocity: Velocity }

module Parser =
  let private parseLine (line: string) =
    match line.Split([| ','; ' '; '@' |], StringSplitOptions.RemoveEmptyEntries) with
    | [| a; b; c; d; e; f |] ->
      { position =
          { x = float a
            y = float b
            z = float c }
        velocity =
          { x = float d
            y = float e
            z = float f } }
      |> Some
    | _ ->
      None

  let printHailstone (h: Hailstone) =
    $"{h.position.x}, {h.position.y}, {h.position.z} @ {h.velocity.x}, {h.velocity.y}, {h.velocity.z}"

  let parseLines (lines: string[]) =
    Array.choose parseLine lines |> List.ofArray

// y(t) = at + b
// x(t) = ct + d
// y = mx + c
type Func =
  { m: float
    c: float
    f: float -> float
    t: float -> float }

let inferYOfX (a, b) (c, d) =
  let t1 x = (x - b) / a
  let m = c / a
  let C = d - (c * b / a)
  let f x = m * x + C

  { m = m; c = C; f = f; t = t1 }

let funcFromHailstone h =
  inferYOfX (h.velocity.x, h.position.x) (h.velocity.y, h.position.y)

let findCrossing f g =
  let f = funcFromHailstone f
  let g = funcFromHailstone g

  if f.c = g.c then
    Some(0., f.c)
  elif f.m = g.m then
    None
  else
    let x = (g.c - f.c) / (f.m - g.m)
    if (f.t x < 0.0 || g.t x < 0.0) then
      None
    else
      Some(x, f.f x)
let intersectsWithinTestArea (min, max) h1 h2 =
  match findCrossing h1 h2 with
  | Some(x, y) when x >= min && x <= max && y >= min && y <= max -> true
  | _ -> false


let pairs xs =
  let rec inner acc =
    function
    | [] -> acc
    | x :: xs -> inner (List.map (fun y -> (x, y)) xs |> List.append acc) xs

  inner [] xs

let howManyIntersect testArea hailstones =
  let ps = pairs hailstones

  let rec inner count i =
    match ps |> List.tryItem i with
    | Some(h1, h2) when intersectsWithinTestArea testArea h1 h2 -> inner (count + 1) (i + 1)
    | Some _ -> inner count (i + 1)
    | None -> count

  inner 0 0

// let partOne = Parser.parseLines >> howManyIntersect (7.0, 27.0)
let partOne = Parser.parseLines >> howManyIntersect (200000000000000.0, 400000000000000.0)

input |> partOne |> printfn "Part 1: %d"
