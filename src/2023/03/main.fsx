open System

module Data =
  let private readLines () =
    stdin.ReadToEnd() |> (fun s -> s.Split "\n")

  let data = readLines ()

  let toStringArray (line: string) = line.ToCharArray() |> Array.map string


module GridUtils =
  let gridMaxima input =
    match input |> Array.tryItem 0 with
    | None -> 0, 0
    | Some line -> (input |> Array.length) - 1, (line |> String.length) - 1

  let adjacentCoords (x, y) (maxX, maxY) =
    [ x + 1, y
      x + 1, y - 1
      x, y - 1
      x - 1, y - 1
      x - 1, y
      x - 1, y + 1
      x, y + 1
      x + 1, y + 1 ]
    |> List.filter (fun (x, y) -> x >= 0 && x <= maxX && y >= 0 && y <= maxY)

  let lookup (grid: string[]) (i, j) =
    match grid |> Array.tryItem i with
    | None -> None
    | Some line -> line.ToCharArray() |> Array.tryItem j

let charIsSymbol (c: char) =
  match c with
  | '.' -> false
  | ch when Char.IsLetterOrDigit(ch) -> false
  | _ -> true

let anyAdjacentSymbols (lines: string[]) i jStart (number: string) (maxX, maxY) =
  let jEnd = jStart + number.Length

  let rec loop j foundSymbol =
    if foundSymbol then
      true
    else
      match j with
      | j when j >= jEnd -> foundSymbol
      | j ->
        let adjacent = GridUtils.adjacentCoords (i, j) (maxX, maxY)

        let anyAdjacent =
          adjacent
          |> List.exists (fun coord ->
            match GridUtils.lookup lines coord with
            | Some ch when charIsSymbol ch -> true
            | _ -> false)

        loop (j + 1) anyAdjacent

  loop jStart false

let numbersInLine (line: string) =
  let rec inner i current acc =
    match line.ToCharArray() |> Array.tryItem i with
    | None ->
      if current <> "" then
        acc @ [ i - current.Length, current ]
      else
        acc
    | Some c when Char.IsDigit(c) -> inner (i + 1) (current + c.ToString()) acc
    | Some _ ->
      match current with
      | "" -> inner (i + 1) "" acc
      | current -> inner (i + 1) "" (acc @ [ (i - current.Length), current ])

  inner 0 "" []

let partOne lines =
  let maxX, maxY = GridUtils.gridMaxima lines

  let rec loop i sum =
    match lines |> Array.tryItem i with
    | None -> sum
    | Some line ->
      let numbers = numbersInLine line

      let totals =
        numbers
        |> List.sumBy (fun (jStart, number) ->
          match anyAdjacentSymbols lines i jStart number (maxX, maxY) with
          | true -> int number
          | false -> 0)

      loop (i + 1) (sum + totals)

  loop 0 0

let gearSymbolsInLine (line: string) =
  let rec inner i acc =
    match line.ToCharArray() |> Array.tryItem i with
    | None -> acc
    | Some c when c = '*' -> inner (i + 1) (acc @ [ i ])
    | _ -> inner (i + 1) acc

  inner 0 []

let findPartsAroundGear (lines: string[]) i j =
  let maxX, maxY = GridUtils.gridMaxima lines

  match lines |> Array.tryItem i with
  | None -> None
  | Some line ->
    let adjacent = GridUtils.adjacentCoords (i, j) (maxX, maxY)

    let adjacentParts =
      adjacent
      |> List.map (fun (x, y) ->
        let ns = numbersInLine lines[x]

        ns
        |> List.filter (fun (start, number) -> (y > start && y <= start + number.Length - 1) || y = start))
      |> List.concat
      |> Set.ofList
      |> Set.toList

    match adjacentParts with
    | [ (_, a); (_, b) ] -> Some(int a, int b)
    | _ -> None

let findPartsAroundGears (lines: string[]) =
  let rec inner i acc =
    match lines |> Array.tryItem i with
    | None -> acc
    | Some line ->
      let possibleGears = gearSymbolsInLine line
      let partsAroundGears = possibleGears |> List.choose (findPartsAroundGear lines i)
      inner (i + 1) (acc @ partsAroundGears)

  inner 0 []

let partTwo lines =
  let gears = findPartsAroundGears lines
  gears |> List.sumBy (fun (a, b) -> a * b)

Data.data |> partOne |> printfn "Part I: %i"
Data.data |> partTwo |> printfn "Part II: %i"
