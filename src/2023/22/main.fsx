#load "../../utils/Utils.fsx"

open Utils

let input = Input.readLines ()

type Coord = int * int * int

module Coord =
  let x (a, _, _) = a
  let y (_, b, _) = b
  let z (_, _, c) = c


type Brick =
  { id: string; Start: Coord; End: Coord }

module Brick =
  let private intersects (a: Brick) (b: Brick) =
    let x1, y1, _ = a.Start
    let x2, y2, _ = a.End
    let x1', y1', _ = b.Start
    let x2', y2', _ = b.End

    x1' <= x2 && x2' >= x1 && y1' <= y2 && y2' >= y1

  let private supported bricks brick =
    brick.Start |> Coord.z = 1
    || bricks
       |> Array.exists (fun b -> Coord.z b.End = Coord.z brick.Start - 1 && intersects brick b)

  let private fall brick =
    let x1, y1, z1 = brick.Start
    let x2, y2, z2 = brick.End

    { id = brick.id
      Start = (x1, y1, z1 - 1)
      End = (x2, y2, z2 - 1) }

  let rec simulateGravity bricks =
    let alreadyFallen =
      bricks
      |> Array.map (fun b ->
        match supported bricks b with
        | true -> b
        | false -> fall b)

    if alreadyFallen = bricks then
      bricks
    else
      simulateGravity alreadyFallen

  let getSupportingBricks bricks brick =
    bricks
    |> Array.filter (fun b -> b <> brick && (Coord.z b.End = Coord.z brick.Start - 1) && intersects brick b)

  let bricksToSupports bricks =
    bricks
    |> Array.fold
      (fun map brick ->
        let supports = getSupportingBricks bricks brick
        map |> Map.add brick supports)
      Map.empty

  let supportsToBricks bricks =
    let bToS = bricksToSupports bricks

    bToS
    |> Map.fold
      (fun map brick supports ->
        supports
        |> Array.fold
          (fun m support ->
            match m |> Map.tryFind support with
            | Some bricks -> m |> Map.add support (Array.append bricks [| brick |])
            | None -> m |> Map.add support [| brick |])
          map)
      Map.empty


  let countHowManyWouldFall bricks =
    bricks
    |> Array.sumBy (fun brick ->
      let otherBricks = bricks |> Array.filter (fun b -> b <> brick)

      let fallenBricks = simulateGravity otherBricks

      fallenBricks
      |> Array.sumBy (fun b ->
        let original = bricks |> Array.find (fun b' -> b'.id = b.id)
        if original <> b then 1 else 0))

  let countBricksThatCanBeDisintegrated bricks =
    let brickSupports = bricksToSupports bricks
    let supportBricks = supportsToBricks bricks

    bricks
    |> Array.filter (fun brick ->
      match supportBricks |> Map.tryFind brick with
      | Some supports ->

        supports
        |> Array.forall (fun b ->
          match brickSupports |> Map.tryFind b with
          | Some xs -> xs |> Array.filter ((<>) brick) |> Array.length > 0
          | None -> false // should never happen
        )
      // doesn't support anything, so can be disintegrated
      | None -> true

    )
    |> Array.length

module Parser =
  let private parseLine (i: int, line: string) =
    match line.Split([| ','; '~' |]) with
    | xs when xs.Length = 6 ->
      match xs |> Array.chunkBySize 3 with
      | [| xs; ys |] ->
        let start = xs |> Array.map int
        let end' = ys |> Array.map int

        Some
          { id = ('A' + char i) |> string
            Start = (start[0], start[1], start[2])
            End = (end'[0], end'[1], end'[2]) }
      | _ -> None
    | _ -> None

  let parse (lines: string[]) =
    lines |> Array.indexed |> Array.choose parseLine

let partOne input =
  let fallenBricks = input |> Parser.parse |> Brick.simulateGravity
  fallenBricks |> Brick.countBricksThatCanBeDisintegrated

let partTwo input =
  let fallenBricks = input |> Parser.parse |> Brick.simulateGravity
  fallenBricks |> Brick.countHowManyWouldFall


input |> partOne |> printfn "Part one: %A"
input |> partTwo |> printfn "Part two: %A"
