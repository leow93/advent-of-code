open System.Text.RegularExpressions

let readFile = System.IO.File.ReadAllLines

type Round = { reds: int; blues: int; greens: int }
type Game = { id: int; rounds: Round[] }

let split (sep: string) (str: string) = str.Split(sep)

let parseCubeCount (s: string) =
  let regex = "(\d+) (\D+)"

  let matches = Regex.Matches(s, regex)
  let m = matches[0]
  let n = int m.Groups[1].Value
  let colour = m.Groups[2].Value

  match colour with
  | "red" -> { reds = n; blues = 0; greens = 0 }
  | "blue" -> { reds = 0; blues = n; greens = 0 }
  | "green" -> { reds = 0; blues = 0; greens = n }
  | _ -> failwith "invalid colour"

let parseRound (roundStr: string) : Round =
  match split "," (roundStr.Trim()) with
  | [||] -> { reds = 0; blues = 0; greens = 0 }
  | xs ->
    xs
    |> Array.fold
      (fun state x ->
        let cubeCount = parseCubeCount x

        { reds = state.reds + cubeCount.reds
          blues = state.blues + cubeCount.blues
          greens = state.greens + cubeCount.greens })
      { reds = 0; blues = 0; greens = 0 }

let parseLine (line: string) =
  match split ":" line with
  | [| gameX; rounds |] ->
    let id =
      match split "Game " gameX with
      | [| _; id |] -> int id
      | _ -> failwith "invalid game id"

    let rounds = rounds |> split ";" |> Array.map parseRound
    Some { id = id; rounds = rounds }
  | _ -> None

type TotalReds = Red of int
type TotalBlues = Blue of int
type TotalGreens = Green of int
type Totals = TotalReds * TotalBlues * TotalGreens

let findPossibleGames games (totalCubes: Totals) =
  let totalReds, totalBlues, totalGreens =
    match totalCubes with
    | Red r, Blue b, Green g -> r, b, g

  games
  |> Array.filter (fun game ->
    game.rounds
    |> Array.forall (fun round ->
      round.reds <= totalReds
      && round.blues <= totalBlues
      && round.greens <= totalGreens))

let parseInput = Array.choose parseLine

let partOne games totalCubes =
  findPossibleGames games totalCubes |> Array.sumBy (fun x -> x.id)

let testInput = readFile "./test.txt" |> parseInput
let realInput = readFile "./data.txt" |> parseInput
let totalCubes = Red 12, Blue 14, Green 13
partOne testInput totalCubes |> printfn "part one (test): %i"
partOne realInput totalCubes |> printfn "part one: %i"

let partTwo games =
  games
  |> Array.sumBy (fun game ->
    let maxReds = game.rounds |> Array.maxBy (fun round -> round.reds)
    let maxBlues = game.rounds |> Array.maxBy (fun round -> round.blues)
    let maxGreens = game.rounds |> Array.maxBy (fun round -> round.greens)
    maxReds.reds * maxBlues.blues * maxGreens.greens)

partTwo testInput |> printfn "part two (test): %i"
partTwo realInput |> printfn "part two: %i"
