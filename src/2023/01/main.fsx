#load "../../utils/Utils.fsx"

open System
let data = Utils.Input.readLines ()

let toStringArray (line: string) = line.ToCharArray() |> Array.map string
let toStringList = toStringArray >> List.ofArray

module PartOne =
  let parse (line: string) =
    line
    |> toStringList
    |> List.choose (fun x ->
      match Int32.TryParse x with
      | true, n -> Some(n)
      | false, _ -> None)
    |> (fun xs -> xs |> List.head, xs |> List.last)

module PartTwo =
  let private wordMap =
    [ "one", 1
      "two", 2
      "three", 3
      "four", 4
      "five", 5
      "six", 6
      "seven", 7
      "eight", 8
      "nine", 9 ]
    |> Map.ofList

  let parse (line: string) =
    let words = wordMap |> Map.keys
    let chars = line |> toStringArray

    let rec inner i first last =
      match chars |> Array.tryItem i with
      | None when first = None || last = None -> failwith "Unexpected failure."
      | None -> first.Value, last.Value
      | Some char ->
        match Int32.TryParse(char) with
        | true, n when first = None -> inner (i + 1) (Some n) (Some n)
        | true, n -> inner (i + 1) first (Some n)
        | false, _ ->
          // must be a word
          match line |> toStringArray |> Array.tryItem (i + 1) with
          | None -> inner (i + 1) first last
          | Some nextChar ->
            match words |> Seq.tryFind (fun word -> word.StartsWith(char + nextChar)) with
            | None -> inner (i + 1) first last
            | Some word ->
              if line[i .. i + word.Length - 1] <> word then
                inner (i + 1) first last
              else
                let n = wordMap[word]

                match first with
                | None -> inner (i + 1) (Some n) (Some n)
                | x -> inner (i + 1) x (Some n)

    inner 0 None None


let solve f (lines: string array) =
  lines
  |> Array.sumBy (fun line ->
    let xs = f line
    let head, last = xs
    (head * 10) + last)


let partOne = solve PartOne.parse

let partTwo = solve PartTwo.parse

partOne data |> printfn "Part I: %A"
partTwo data |> printfn "Part II: %A"
