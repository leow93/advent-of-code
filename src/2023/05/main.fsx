#load "../../utils/Utils.fsx"

open System
open Utils

type MapFn = int64 -> int64

type Seeds = int64 seq

type Almanac =
  { seedToSoil: MapFn
    soilToFertilizer: MapFn
    fertilizerToWater: MapFn
    waterToLight: MapFn
    lightToTemperature: MapFn
    temperatureToHumidity: MapFn
    humidityToLocation: MapFn }

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
type Parser(text: string) =
  let maps = text.Split("\n\n") |> List.ofArray

  let parseMapFn (str: string) =
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
      let almanac =
        { seedToSoil = parseMapFn b
          soilToFertilizer = parseMapFn c
          fertilizerToWater = parseMapFn d
          waterToLight = parseMapFn e
          lightToTemperature = parseMapFn f
          temperatureToHumidity = parseMapFn g
          humidityToLocation = parseMapFn h }

      Some(almanac, parseSeeds seeds, parseSeedsRanges seeds)
    | _ -> None

  let fst (a, _, _) = a
  let snd (_, b, _) = b
  let trd (_, _, c) = c

  member this.Almanac = parsedMaps |> Option.map fst |> Option.get
  member this.Seeds = parsedMaps |> Option.map snd |> Option.get
  member this.SeedRanges = parsedMaps |> Option.map trd |> Option.get

let seedToLocation (alm: Almanac) =
  alm.seedToSoil
  >> alm.soilToFertilizer
  >> alm.fertilizerToWater
  >> alm.waterToLight
  >> alm.lightToTemperature
  >> alm.temperatureToHumidity
  >> alm.humidityToLocation

let parsed = Parser(Input.text)
let almanac = parsed.Almanac
let seeds = parsed.Seeds
let seedRanges = parsed.SeedRanges

let partOne () =
  seeds |> Seq.map (seedToLocation almanac) |> Seq.min

let partTwo () =
  let mutable result = Int64.MaxValue
  let total = seedRanges |> Seq.sumBy snd
  let mutable i = 0L
  for range in seedRanges do
    let start, count = range
    for seed in start .. start + count - 1L do
      let loc = seedToLocation almanac seed
      i <- i + 1L
      if i % 1_000_000L = 0L then
        printfn "done: %d%%" (100L * i / total)
      if loc < result then
        result <- loc


  result

partOne () |> printfn "Part one: %d"
partTwo () |> printfn "Part two: %d"
