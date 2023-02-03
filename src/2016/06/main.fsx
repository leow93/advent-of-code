open System

let join (list: char []) =
  list
  |> Array.fold (fun acc s -> acc + (string s)) ""

let parseInput (input: string) =
  input.Split '\n'
  |> Array.filter (not << String.IsNullOrWhiteSpace)
  |> Array.map (fun s -> s.ToCharArray())
  |> Array.transpose
  |> Array.map join

let countLetters (s: string) =
  s
  |> Seq.fold
       (fun map letter ->
         match map |> Map.tryFind letter with
         | Some _ -> map
         | None ->
           let count =
             s
             |> Seq.filter (fun ch -> ch = letter)
             |> Seq.length

           map |> Map.add letter count)
       Map.empty

let mostCommonLetter (s: string) =
  s
  |> countLetters
  |> Map.toArray
  |> Array.sortByDescending snd
  |> Array.head
  |> fst

let leastCommonLetter (s: string) =
  s
  |> countLetters
  |> Map.toArray
  |> Array.sortBy snd
  |> Array.head
  |> fst

let input =
  System.IO.File.ReadAllText "./input.txt"

let partOne =
  input
  |> parseInput
  |> Array.map mostCommonLetter
  |> join

partOne |> printfn "Part I: %A"

let partTwo =
  input
  |> parseInput
  |> Array.map leastCommonLetter
  |> join

partTwo |> printfn "Part II: %A"
