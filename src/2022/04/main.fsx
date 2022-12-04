let readLines = System.IO.File.ReadLines
let split (separator: string) (s: string) = s.Split(separator)
let range a b = Array.init (b - a + 1) (fun i -> a + i)

let toRange s =
  match split "-" s with
  | [| a; b |] -> Some(range (int a) (int b))
  | _ -> None

let toTuple line =
  match split "," line with
  | [| a; b |] ->
    match toRange a, toRange b with
    | Some a, Some b -> Some(a, b)
    | _ -> None
  | _ -> None

let partOne file =
  readLines file
  |> Array.ofSeq
  |> Array.choose toTuple
  |> Array.sumBy (fun (a, b) ->
    let setA = a |> Set.ofArray
    let setB = b |> Set.ofArray

    if setA |> Set.isSuperset setB
       || setB |> Set.isSuperset setA then
      1
    else
      0)

let partTwo file =
  readLines file
  |> Array.ofSeq
  |> Array.choose toTuple
  |> Array.sumBy (fun (a, b) ->
    let intersection =
      (Set.ofArray a) |> Set.intersect (Set.ofArray b)

    match intersection.IsEmpty with
    | true -> 0
    | false -> 1)

printfn "Test"
partOne "./test.txt" |> printf "Part I: %A\n"
partTwo "./test.txt" |> printf "Part II: %A\n"

printfn "\nActual"
partOne "./data.txt" |> printf "Part I: %A\n"
partTwo "./data.txt" |> printf "Part II: %A\n"
