open System.Text.RegularExpressions

let readLines = System.IO.File.ReadAllLines

let regex =
  "(\w+) would (\w+) (\d+) happiness units by sitting next to (\w+)"

type GainOrLose =
  | Gain of int
  | Lose of int

type Info = string * GainOrLose * string

module Info =
  let firstPerson ((x, _, _): Info) = x
  let secondPerson ((_, _, x): Info) = x


let parseLine line : Info =
  Regex.Match(line, regex).Groups
  |> (fun x ->
    let g =
      match x[2].Value with
      | "gain" -> Gain(x[3].Value |> int)
      | _ -> Lose(x[3].Value |> int)

    x[1].Value, g, x[4].Value)

let parseLines lines = lines |> Array.map parseLine

let testData =
  readLines "./test.txt" |> parseLines

let actualData =
  readLines "./data.txt" |> parseLines

let rec distribute e =
  function
  | [] -> [ [ e ] ]
  | x :: xs' as xs ->
    (e :: xs)
    :: [ for xs in distribute e xs' -> x :: xs ]

let rec permute =
  function
  | [] -> [ [] ]
  | e :: xs -> List.collect (distribute e) (permute xs)

let getPeople infos =
  infos
  |> Array.fold (fun s info -> s |> Set.add (Info.firstPerson info)) Set.empty


type AttributeMap = Map<string * string, GainOrLose>

module AttributeMap =

  let private key ((a, _, b): Info) = (a, b)

  let build infos : AttributeMap =
    infos
    |> Array.fold
         (fun map info ->
           let _, g, _ = info
           map |> Map.add (info |> key) g)
         Map.empty

  let tryFind (a, b) (map: AttributeMap) =
    let rec inner (a, b) map attempt =
      match map |> Map.tryFind (a, b) with
      | Some x -> Some x
      | None when attempt = 2 -> None
      | None -> inner (b, a) map (attempt + 1)

    inner (a, b) map 1

let getPermutations (people: Set<string>) = people |> List.ofSeq |> permute

let getTotalHappiness (attributes: AttributeMap) (permutations: string list list) =
  permutations
  |> List.fold
       (fun totalHappiness permutation ->
         let total =
           permutation
           |> List.indexed
           |> List.fold
                (fun total (i, person) ->
                  let left =
                    match i with
                    | 0 -> permutation[permutation.Length - 1]
                    | i -> permutation[i - 1]

                  let right =
                    match i = (permutation.Length - 1) with
                    | true -> permutation[0]
                    | false -> permutation[i + 1]

                  match (attributes |> AttributeMap.tryFind (person, left), attributes |> AttributeMap.tryFind (person, right)) with
                  | Some (Lose l), Some (Lose r) -> total - l - r
                  | Some (Lose l), Some (Gain r) -> total - l + r
                  | Some (Gain l), Some (Lose r) -> total + l - r
                  | Some (Gain l), Some (Gain r) -> total + l + r
                  | Some (Gain l), None -> total + l
                  | Some (Lose l), None -> total - l
                  | None, Some (Gain l) -> total + l
                  | None, Some (Lose l) -> total - l
                  | None, None -> total)
                0

         if total > totalHappiness then
           total
         else
           totalHappiness)
       0

let partOne =
  let people = getPeople actualData

  let attributes =
    AttributeMap.build actualData

  let permutations = getPermutations people

  getTotalHappiness attributes permutations

let partTwo =
  let people = getPeople actualData

  let data =
    people
    |> Set.fold
         (fun acc person ->
           acc
           |> Array.append [| ("Leo", Gain 0, person)
                              (person, Gain 0, "Leo") |])
         actualData

  let people = getPeople data
  let attributes = AttributeMap.build data

  let permutations = getPermutations people

  getTotalHappiness attributes permutations

partOne |> printfn "Part I: %i"
partTwo |> printfn "Part II: %i"
