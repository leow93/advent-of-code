#load "../../utils/Utils.fsx"

open System
open System.Diagnostics
open Utils

type Seeds = int64 seq

type Spec =
  { destination: int64
    source: int64
    range: int64 }

let buildMapFn specs x =
  let spec = specs |> Array.tryFind (fun s -> x >= s.source && x < s.source + s.range)

  match spec with
  | None -> x
  | Some spec ->
    let delta = x - spec.source
    spec.destination + delta

let parseSeeds (str: string) =
  match str.Split(": ") with
  | [| _; numbers |] -> numbers.Split(" ") |> Array.map int64 |> Seq.ofArray
  | _ -> Seq.empty

let parseSeedsRanges (str: string) =
  let numbers =
    match str.Split(": ") with
    | [| _; numbers |] -> numbers.Split(" ") |> Array.map int64
    | _ -> [||]

  numbers
  |> Array.chunkBySize 2
  |> Array.choose (fun xs ->
    match xs with
    | [| a; b |] -> Some(a, b)
    | _ -> None)

type MapFn = int64 -> int64

type Parser(text: string) =
  let maps = text.Split("\n\n") |> List.ofArray

  let parseMapFn (str: string) : MapFn =
    str.Split("\n")
    |> Array.skip 1
    |> Array.choose (fun line ->
      match line.Split(" ") |> Array.map int64 with
      | [| x; y; z |] ->
        let spec =
          { destination = x
            source = y
            range = z }

        Some spec
      | _ -> None)
    |> buildMapFn

  let parsedMaps =
    match maps with
    | [ seeds; b; c; d; e; f; g; h ] ->
      let seedToSoil = parseMapFn b
      let soilToFertilizer = parseMapFn c
      let fertilizerToWater = parseMapFn d
      let waterToLight = parseMapFn e
      let lightToTemperature = parseMapFn f
      let temperatureToHumidity = parseMapFn g
      let humidityToLocation = parseMapFn h

      let seedToLocation =
        seedToSoil
        >> soilToFertilizer
        >> fertilizerToWater
        >> waterToLight
        >> lightToTemperature
        >> temperatureToHumidity
        >> humidityToLocation

      Some(seedToLocation, parseSeeds seeds, parseSeedsRanges seeds)
    | _ -> None

  let fst (a, _, _) = a
  let snd (_, b, _) = b
  let trd (_, _, c) = c

  member this.Almanac = parsedMaps |> Option.map fst |> Option.get
  member this.Seeds = parsedMaps |> Option.map snd |> Option.get
  member this.SeedRanges = parsedMaps |> Option.map trd |> Option.get

let parsed = Parser(Input.text)
let seedToLocation = parsed.Almanac
let seeds = parsed.Seeds
let seedRanges = parsed.SeedRanges

let time description f =
  let sw = Stopwatch.StartNew()
  let result = f ()
  sw.Stop()
  printfn "%s: %dms" description sw.ElapsedMilliseconds
  result

let partOne () =
  seeds |> Seq.map seedToLocation |> Seq.min

let partTwo () =
  let results =
    seedRanges
    |> Seq.map (fun range ->
      async {
        let start, count = range

        let rec inner i curr =
          if i = start + count then
            curr
          else
            let loc = seedToLocation i

            if loc < curr then
              inner (i + 1L) loc
            else
              inner (i + 1L) curr

        return inner start Int64.MaxValue
      })
    |> Async.Parallel
    |> Async.RunSynchronously

  results |> Seq.min

time "Part one" partOne |> printfn "%d"
time "Part two" partTwo |> printfn "%d"
