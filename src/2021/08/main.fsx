let input =
  System.IO.File.ReadAllLines "./data.txt"

let solve (input: string []) =
  let easy_nb_segments = [ 2; 3; 4; 7 ]

  input
  |> Array.collect (fun s ->
    s.Split(" | ").[1].Split(" ")
    |> Array.map Seq.length)
  |> Array.filter (fun n -> easy_nb_segments |> Seq.contains n)
  |> Array.length

let partOne =
  solve input |> printfn "Part I: %i"

let partTwo () =
  let parse (line: string) =
    let parseWords (t: string) = t.Split(" ")
    let [| signalPatternsString; outputDigitsString |] = line.Split(" | ")

    let signalPatterns =
        signalPatternsString
        |> parseWords
        |> Array.map Set.ofSeq

    let outputDigits =
        outputDigitsString
        |> parseWords
        |> Array.map Set.ofSeq

    (signalPatterns, outputDigits)

  let calculateOutput (patterns, output) =
        //Lookup helpers
    let ofUniqueLength n segments =
        segments
        |> Seq.find (fun s -> s |> Seq.length = n)

    let ofLength n segments =
        segments
        |> Seq.filter (fun s -> s |> Seq.length = n)
    //ADT operations
    let minus a b = Set.difference b a
    let containsSegment = Set.isSubset 

    //Let's figure out which segment pattern represents which digit
    //Start with the easy digits of unique segment lengths
    let one = patterns |> ofUniqueLength 2
    let four = patterns |> ofUniqueLength 4
    let seven = patterns |> ofUniqueLength 3
    let eight = patterns |> ofUniqueLength 7

    //Complex digits need some deduction more deduction, we'll use segments CF and BD to figure them out
    let segmentsCF = one
    let segmentsBD = four |> minus segmentsCF

    let digitsWithFiveSegments = patterns |> ofLength 5

    let three =
        digitsWithFiveSegments
        |> Seq.find (containsSegment segmentsCF)

    let five =
        digitsWithFiveSegments
        |> Seq.find (containsSegment segmentsBD)

    let two =
        digitsWithFiveSegments
        |> Seq.except [ three; five ]
        |> Seq.head

    let digitsWithSixSegments = patterns |> ofLength 6

    let six =
        digitsWithSixSegments
        |> Seq.find (containsSegment segmentsCF >> not)

    let nine =
        digitsWithSixSegments
        |> Seq.filter (containsSegment segmentsCF)
        |> Seq.filter (containsSegment segmentsBD)
        |> Seq.head

    let zero =
        digitsWithSixSegments
        |> Seq.except [ nine; six ]
        |> Seq.head

    //YAY! Let's build a lookup table mapping segments to digits now
    let lookup =
        [ (zero, 0)
          (one, 1)
          (two, 2)
          (three, 3)
          (four, 4)
          (five, 5)
          (six, 6)
          (seven, 7)
          (eight, 8)
          (nine, 9) ]
        |> Map.ofSeq

    let outputDigit =
        output
        |> Seq.map (fun digit -> lookup |> Map.find digit)
        |> Seq.map string
        |> String.concat ""
        |> int

    outputDigit

  input
    |> Array.map parse
    |> Array.map calculateOutput
    |> Array.sum


partTwo() |> printfn "Part II: %i"