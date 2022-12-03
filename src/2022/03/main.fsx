open System
let readLines = System.IO.File.ReadLines
type Bag(contents: string) =
  let length = contents.Length

  let firstCompartment =
    contents.Substring(0, length / 2) |> Set.ofSeq

  let secondCompartment =
    contents.Substring(length / 2) |> Set.ofSeq

  member _.SharedItem() =
    firstCompartment
    |> Seq.find (fun item -> secondCompartment |> Set.contains item)

type ItemPriority(char: char) =
  let alphabet = [ 'a' .. 'z' ]

  let position char =
    (alphabet
     |> List.findIndex (fun x -> x = (char |> Char.ToLower)))
    + 1

  member _.Priority() =
    match Char.IsUpper char with
    | true -> position char + 26
    | false -> position char

let findBadge (one, two, three) =
  one
  |> Set.ofArray
  |> Set.intersect (Set.ofArray two)
  |> Set.intersect (Set.ofArray three)
  |> Seq.head

let partOne file =
  readLines file
  |> Seq.map (
    Bag
    >> fun b -> b.SharedItem()
    >> ItemPriority
    >> fun x -> x.Priority()
  )
  |> Seq.sum

let triples =
  function
  | [| one; two; three |] -> Some(one, two, three)
  | _ -> None

let partTwo file =
  readLines file
  |> Seq.map (fun s -> s.ToCharArray())
  |> Seq.chunkBySize 3
  |> Seq.choose triples
  |> Seq.map (
    findBadge
    >> ItemPriority
    >> fun x -> x.Priority()
  )
  |> Seq.sum

printfn "Test"
partOne "./test.txt" |> printf "Part I: %i\n"
partTwo "./test.txt" |> printf "Part II: %i\n"

printfn "\nActual"
partOne "./data.txt" |> printf "Part I: %i\n"
partTwo "./data.txt" |> printf "Part II: %i\n"