#load "../../utils/Utils.fsx"

open System
open Utils

type Race = { time: int64; distanceRecord: int64 }

module Parse =
  let private tryParseInt (s: string) =
    match Int64.TryParse(s) with
    | true, i -> Some i
    | _ -> None

  let private trim (s: string) = s.Trim()
  let private split (sep: string) (s: string) = s.Split(sep)

  let private concat (xs: string[]) = xs |> Array.fold (+) ""

  let parseAsSingleRace (lines: string) =
    match lines |> split "\n" with
    | [| time; distance |] ->
      let timeString = time.Substring(5) |> trim |> split " " |> concat

      let distanceString = distance.Substring(9) |> trim |> split " " |> concat

      { time = int64 timeString
        distanceRecord = int64 distanceString }
    | _ -> failwith "Invalid input"

  let parseAsMultipleRaces (lines: string) =
    match lines |> split "\n" with
    | [| times; distances |] ->
      let times =
        times.Substring(5) |> trim |> split " " |> Array.choose (trim >> tryParseInt)

      let distances =
        distances.Substring(9)
        |> trim
        |> split " "
        |> Array.choose (trim >> tryParseInt)

      Array.zip times distances
      |> Array.map (fun (time, distance) ->
        { time = int64 time
          distanceRecord = int64 distance })
    | _ -> [||]

let input = Input.readText ()

let countWaysToWinBruteForce race =
  let distanceRecord = race.distanceRecord

  let rec inner count time =
    if time > race.time then
      count
    else
      let remainingTime = race.time - time
      let d = remainingTime * time

      if d > distanceRecord then
        inner (count + 1L) (time + 1L)
      else
        inner count (time + 1L)

  inner 0L 0L

let solveQuadratic a b c =
  let x1 = (-b + sqrt ((b * b) - 4.0 * a * c)) / (2.0 * a)
  let x2 = (-b - sqrt ((b * b) - 4.0 * a * c)) / (2.0 * a)
  x1, x2

let countWaysToWin race =
  let x1, x2 =
    solveQuadratic 1 (-1.0 * (race.time |> float)) (race.distanceRecord |> float)

  let floor (x: float) = Math.Floor(x) |> int64

  floor x1 - floor x2

let multiply xs = xs |> Array.fold (*) 1L

let partOneBruteForce input =
  input
  |> Parse.parseAsMultipleRaces
  |> Array.map countWaysToWinBruteForce
  |> multiply

let partTwoBruteForce input =
  input |> Parse.parseAsSingleRace |> countWaysToWinBruteForce

let partOne input =
  input |> Parse.parseAsMultipleRaces |> Array.map countWaysToWin |> multiply

let partTwo input =
  input |> Parse.parseAsSingleRace |> countWaysToWin

input |> partOneBruteForce |> printfn "Part 1 brute force: %d"
input |> partTwoBruteForce |> printfn "Part 2 brute force: %d"

input |> partOne |> printfn "Part 1: %d"
input |> partTwo |> printfn "Part 2: %d"
