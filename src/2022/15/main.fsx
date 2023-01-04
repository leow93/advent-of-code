open System.Text.RegularExpressions

let readLines = System.IO.File.ReadLines

type Sensor =
  { sensor: int * int
    beacon: int * int
    distance: int }

let manhattanDistance (x1, y1) (x2, y2) = abs (x2 - x1) + abs (y2 - y1)

let parseLine line =
  Regex.Matches(line, "(-?\d+)")
  |> Seq.map (fun x -> int x.Groups[0].Value)
  |> Array.ofSeq
  |> (fun x ->
    let sensor = (x[0], x[1])
    let beacon = (x[2], x[3])

    { sensor = sensor
      beacon = beacon
      distance = manhattanDistance sensor beacon })

let parseData file =
  readLines file |> Seq.map parseLine |> Array.ofSeq

let closerToRow row sensor =
  abs (row - (sensor.sensor |> snd)) < sensor.distance

let beaconsAtRow row sensor = (sensor.beacon |> snd) = row

let coverageAtRow row sensor =
  let r =
    abs (
      sensor.distance
      - abs (row - (sensor.sensor |> snd))
    )

  (sensor.sensor |> fst) - r, (sensor.sensor |> fst) + r

let maxCoverageAtRow maxY row sensor =
  let r =
    abs (
      sensor.distance
      - abs (row - (sensor.sensor |> snd))
    )

  max 0 ((sensor.sensor |> fst) - r), min maxY ((sensor.sensor |> fst) + r)

let sumRanges sum (a, b) = sum + abs (b + 1 - a)

let merge (ranges: seq<int * int>) =
  let sort (ax, ay) (bx, by) =
    match ax - bx with
    | 0 -> ay - by
    | x -> x

  let fold acc (a0, a1) =
    match acc with
    | [] -> [ a0, a1 ]
    | (l0, l1) :: tail when a0 <= (l1 + 1) && l1 < a1 -> (l0, a1) :: tail
    | (_, l1) :: _ when a0 <= (l1 + 1) -> acc
    | _ -> (a0, a1) :: acc

  ranges
  |> Seq.sortWith sort
  |> Seq.fold fold List.empty

let partOne (row: int) (data: Sensor array) =
  let pointsInRange =
    data
    |> Seq.filter (closerToRow row)
    |> Seq.map (coverageAtRow row)
    |> merge
    |> Seq.fold sumRanges 0

  let beaconsCount =
    data
    |> Seq.filter (beaconsAtRow row)
    |> Seq.map (fun sensor -> sensor.beacon |> fst)
    |> Seq.distinct
    |> Seq.length

  pointsInRange - beaconsCount

let partTwo maxY (data: Sensor array) =
  let coverageAtRow' = maxCoverageAtRow maxY

  [ maxY .. -1 .. 0 ]
  |> Seq.pick (fun row ->
    data
    |> Seq.filter (closerToRow row)
    |> Seq.map (coverageAtRow' row)
    |> merge
    |> (fun s ->
      match s |> Seq.length with
      | n when n > 1 ->
        s
        |> Seq.head
        |> fst
        |> (fun xs -> Some((int64 xs - 1L) * 4000000L + int64 row))
      | _ -> None))

let testData = parseData "./test.txt"

let actualData = parseData "./data.txt"

testData
|> partOne 10
|> printfn "Part I (test): %i"

actualData
|> partOne 2000000
|> printfn "Part I (real): %i"

testData
|> partTwo 20
|> printfn "Part II (test): %i"

actualData
|> partTwo 4000000
|> printfn "Part II (test): %i"
