#load "../../utils/Utils.fsx"

open System.Collections.Generic
open Utils

type MapFn = int64 -> int64

type Almanac =
  { seeds: int64[]
    seedToSoil: MapFn
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


type Parser(text: string, parseSeeds: string -> int64 array) =
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
    | [ a; b; c; d; e; f; g; h ] ->
      let almanac =
        { seeds = parseSeeds a
          seedToSoil = parseMapFn b
          soilToFertilizer = parseMapFn c
          fertilizerToWater = parseMapFn d
          waterToLight = parseMapFn e
          lightToTemperature = parseMapFn f
          temperatureToHumidity = parseMapFn g
          humidityToLocation = parseMapFn h }

      Some almanac
    | _ -> None

  member this.Almanac = parsedMaps.Value

let seedToLocation (alm: Almanac) (seed: int64) =
  seed
  |> alm.seedToSoil
  |> alm.soilToFertilizer
  |> alm.fertilizerToWater
  |> alm.waterToLight
  |> alm.lightToTemperature
  |> alm.temperatureToHumidity
  |> alm.humidityToLocation

let solve parseSeeds =
  let almanac = Parser(Input.text, parseSeeds).Almanac

  almanac.seeds |> Array.map (seedToLocation almanac) |> Array.min

let parseSeeds (str: string) =
  match str.Split(": ") with
  | [| _; numbers |] -> numbers.Split(" ") |> Array.map int64
  | _ -> [||]

solve parseSeeds |> printfn "Part one: %d"