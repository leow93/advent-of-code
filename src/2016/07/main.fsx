open System
open Microsoft.FSharp.Collections

type Address =
  { hypernetSequences: string list
    rest: string list }

let join (chars: char seq) = String.Join("", chars)

module Input =
  let parseLine (s: string) =
    match
      s.Split('[', ']')
      |> Array.filter (not << String.IsNullOrWhiteSpace)
      with
    | [||] -> None
    | [| a; hypernetSequence; b |] ->
      { hypernetSequences = [ hypernetSequence ]
        rest = [ a; b ] }
      |> Some
    | [| a; hypernet1; b; hypernet2; c |] ->
      { hypernetSequences = [ hypernet1; hypernet2 ]
        rest = [ a; b; c ] }
      |> Some
    | [| a; h1; b; h2; c; h3; d |] ->
      { hypernetSequences = [ h1; h2; h3 ]
        rest = [ a; b; c; d ] }
      |> Some
    | _ -> None

let containsABBA (s: string) =
  s.ToCharArray()
  |> Array.windowed 4
  |> List.ofArray
  |> List.exists (function
    | [| a; b; c; d |] -> a = d && b = c && a <> b
    | _ -> false)

let findABASequence (s: string) =
  s.ToCharArray()
  |> Array.windowed 3
  |> List.ofArray
  |> List.filter (function
    | [| a; b; c |] -> a = c && a <> b
    | _ -> false)

let supportsTLS address =
  address.rest |> List.exists containsABBA
  && address.hypernetSequences
     |> List.forall (not << containsABBA)

let supportsSSL address =
  let ABASequences =
    address.rest
    |> List.map findABASequence
    |> List.concat

  let BABSequences =
    ABASequences
    |> List.choose (function
      | [| a; b; c |] when a = c -> [| b; a; b |] |> Some
      | _ -> None)

  ABASequences
  |> List.indexed
  |> List.exists (fun (i, _) ->
    let BAB = BABSequences[i]

    address.hypernetSequences
    |> List.exists (fun s -> s.Contains(join BAB)))

let input = System.IO.File.ReadAllLines "./input.txt"

let partOne =
  input
  |> Array.choose Input.parseLine
  |> Array.filter supportsTLS
  |> Array.length

printfn "Part I: %i" partOne

let partTwo =
  input
  |> Array.choose Input.parseLine
  |> Array.filter supportsSSL
  |> Array.length

printfn "Part II: %i" partTwo
