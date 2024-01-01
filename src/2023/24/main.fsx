#load "../../utils/Utils.fsx"

open System
open Utils

let input = Input.readLines ()
type Hailstone = decimal * decimal * decimal * decimal * decimal * decimal
let position (x, y, z, _, _, _) = (x, y, z)
let velocity (_, _, _, x, y, z) = (x, y, z)

let sextuple (xs: decimal[]) =
  (xs[0], xs[1], xs[2], xs[3], xs[4], xs[5])

module Parser =
  let private parseLine (line: string) =
    match line.Split([| ','; ' '; '@' |], StringSplitOptions.RemoveEmptyEntries) with
    | xs when xs.Length = 6 -> xs |> Array.map decimal |> sextuple |> Some
    | _ -> None

  let parseLines (lines: string[]) =
    Array.choose parseLine lines |> List.ofArray

module LinearParametricEquation =
  // y(t) = at + b
  // x(t) = ct + d
  // y = mx + c
  type Func =
    { m: decimal
      c: decimal
      f: decimal -> decimal
      t: decimal -> decimal }

  let inferYOfX (a, b) (c, d) =
    let t1 x = (x - b) / a
    let m = c / a
    let C = d - (c * b / a)
    let f x = m * x + C

    { m = m; c = C; f = f; t = t1 }

  let funcFromHailstone h =
    let x, y, _, vx, vy, _ = h
    inferYOfX (vx, x) (vy, y)

  let findCrossing f g =
    let f = funcFromHailstone f
    let g = funcFromHailstone g

    if f.c = g.c then
      Some(0m, f.c)
    elif f.m = g.m then
      None
    else
      let x = (g.c - f.c) / (f.m - g.m)

      if (f.t x < 0m || g.t x < 0m) then None else Some(x, f.f x)

let intersectsWithinTestArea (min, max) h1 h2 =
  match LinearParametricEquation.findCrossing h1 h2 with
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

let hailstones = input |> Parser.parseLines

let partOne = hailstones |> howManyIntersect (200000000000000m, 400000000000000m)

partOne |> printfn "Part 1: %d"

let xyVelocities =
  Seq.initInfinite id
  |> Seq.map decimal
  |> Seq.collect (fun n ->
    seq {
      for x in 0m .. n do
        x, n - x
        -x, n - x
        x, x - n
        -x, x - n
    })

// Move a hailstone to the rock's frame of reference
let velocityTransformation (vx, vy) hailstone =
  let x, y, z, vx', vy', vz = hailstone
  x, y, z, vx' - vx, vy' - vy, vz


let crossXY (h1, h2) =
  // y = y0 + m (x - x0) => x = (y0b - y0a + ma*x0a - mb*x0b)/(ma - mb)
  let x1, y1, _, vx1, vy1, _ = h1
  let x2, y2, _, vx2, vy2, _ = h2

  if vx1 = 0m || vx2 = 0m then
    None
  else
    let m1 = vy1 / vx1
    let m2 = vy2 / vx2

    if m1 = m2 then
      None
    else
      let x = (y2 - y1 + m1 * x1 - m2 * x2) / (m1 - m2)
      let y = y1 + m1 * (x - x1)
      Some(x, y)

let vx, vy =
  xyVelocities
  |> Seq.find (fun (vx, vy) ->
    let h = hailstones[0]
    let h' = velocityTransformation (vx, vy) h

    let intersections =
      hailstones
      |> List.skip 1
      |> List.take 5
      |> List.map (fun x -> crossXY (h', velocityTransformation (vx, vy) x))

    intersections |> List.forall Option.isSome
    && (intersections
        |> List.map Option.get
        |> List.pairwise
        |> List.forall (fun ((xc1, yc1), (xc2, yc2)) -> abs (xc2 - xc1) < 1m && abs (yc2 - yc1) < 1m)))

let vz h1 h2 xc =
  // p = p0 + v*t => t = (p - p0) / v
  let x1, _, z1, vx1, _, vz1 = h1
  let x2, _, z2, vx2, _, vz2 = h2
  let t1 = (xc - x1) / vx1
  let t2 = (xc - x2) / vx2
  (z1 - z2 + t1 * vz1 - t2 * vz2) / (t1 - t2)

let mapTuple f (x, y) = f x, f y

let partTwo =
  let h0 = hailstones[0]
  let h1 = hailstones[1]

  let h0', h1' = (h0, h1) |> mapTuple (velocityTransformation (vx, vy))
  let xc, yc = crossXY (h0', h1') |> Option.get
  let vz = vz h0' h1' xc
  let x0', _, z0', vx0', _, vz0' = h0'

  (xc, yc, z0' + (xc - x0') * (vz0' - vz) / vx0')
  |> (fun (x, y, z) -> x + y + z |> round)

printfn "Part 2: %A" partTwo
