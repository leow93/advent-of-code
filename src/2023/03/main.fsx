open System

let readLines () =
  stdin.ReadToEnd() |> (fun s -> s.Split "\n")

let data = readLines ()

let toStringArray (line: string) = line.ToCharArray() |> Array.map string

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
        let adjacent = adjacentCoords (i, j) (maxX, maxY)

        let anyAdjacent =
          adjacent
          |> List.exists (fun coord ->
            match lookup lines coord with
            | Some ch when charIsSymbol ch -> true
            | _ -> false)

        loop (j + 1) anyAdjacent

  loop jStart false


let partOne lines =
  let maxX, maxY = gridMaxima lines

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
  
data |> partOne |> printfn "%A"
