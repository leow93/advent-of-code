let readLines = System.IO.File.ReadAllLines

let hasThreeVowels (chars: char array) =
  let vowels = [ 'a'; 'e'; 'i'; 'o'; 'u' ]

  let rec loop vowelsList idx =
    if vowelsList |> List.length >= 3 then
      true
    else
      match chars |> Array.tryItem idx with
      | None -> false
      | Some ch ->
        if vowels |> List.contains ch then
          loop (vowelsList |> List.append [ ch ]) (idx + 1)
        else
          loop vowelsList (idx + 1)

  loop List.empty 0

let containsDoubleLetter (chars: char array) =
  chars
  |> Array.pairwise
  |> Array.filter (fun (a, b) -> a = b)
  |> Array.length > 0

let noBadStrings (chars: char array) =
  let bad =
    [ ('a', 'b')
      ('c', 'd')
      ('p', 'q')
      ('x', 'y') ]

  chars
  |> Array.pairwise
  |> (fun xs ->
    match xs
          |> Array.tryFind (fun pair -> (bad |> List.contains pair))
      with
    | Some _ -> false
    | _ -> true)


let isNicePartOne (s: string) =
  let chars = s.ToCharArray()

  hasThreeVowels chars
  && containsDoubleLetter chars
  && noBadStrings chars

let data = readLines "./data.txt"

let partOne () =
  data
  |> Array.filter isNicePartOne
  |> Array.length

partOne () |> printfn "Part I: %i"

let twoPairsNotOverlapping (chars: char array) =
  chars
  |> Array.pairwise
  |> Array.indexed
  |> (fun xs ->
    xs
    |> Array.exists (fun (i, a) ->
      xs
      |> Array.exists (fun (j, b) -> i <> j && (abs (i - j) > 1) && a = b))
    )

let oneRepeatedLetterWithOneBetween (chars: char array) =
  let rec loop idx =
    match chars |> Array.tryItem idx with
    | None -> false
    | Some a ->
      match chars |> Array.tryItem (idx + 2) with
      | None -> false
      | Some b when a = b -> true
      | Some _ -> loop (idx + 1)
  
  loop 0

let isNicePartTwo (s: string) =
  let chars = s.ToCharArray()

  twoPairsNotOverlapping chars && oneRepeatedLetterWithOneBetween chars

let partTwo () =
  data
  |> Array.filter isNicePartTwo
  |> Array.length
  
partTwo () |> printfn "Part II: %i"
