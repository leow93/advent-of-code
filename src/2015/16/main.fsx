open System.Text.RegularExpressions

let readLines = System.IO.File.ReadAllLines

type Attribute =
  | Children of int
  | Cats of int
  | Samoyeds of int
  | Pomeranians of int
  | Akitas of int
  | Vizslas of int
  | Goldfish of int
  | Trees of int
  | Cars of int
  | Perfumes of int

// type Sue = {
//   children: int option
//   cats: int option
//   samoyeds: int option
//   pomeranians: int option
//   akitas: int option
//   vizslas: int option
//   goldfish: int option
//   trees: int option
//   cars: int option
//   perfumes: int option
// }
type Sue = Attribute * Attribute * Attribute

module Input =
  let private regex =
    "Sue (\d+): (\w+): (\d+), (\w+): (\d+), (\w+): (\d+)"

  let private parseAttribute name count =
    match name with
    | "children" -> Children
    | "cats" -> Cats
    | "samoyeds" -> Samoyeds
    | "pomeranians" -> Pomeranians
    | "akitas" -> Akitas
    | "vizslas" -> Vizslas
    | "goldfish" -> Goldfish
    | "trees" -> Trees
    | "cars" -> Cars
    | "perfumes" -> Perfumes
    | _ -> failwithf "unknown type %s" name
    |> (fun x -> x count)

  let private parseLine (line: string) : int * Sue =
    Regex.Match(line, regex).Groups
    |> (fun groups ->
      let id = groups[1].Value |> int

      let first =
        parseAttribute groups[2].Value (groups[3].Value |> int)

      let second =
        parseAttribute groups[4].Value (groups[5].Value |> int)

      let third =
        parseAttribute groups[6].Value (groups[7].Value |> int)

      id, (first, second, third))

  let parseLines lines = lines |> Array.map parseLine


type Optimal =
  { children: int
    cats: int
    samoyeds: int
    pomeranians: int
    akitas: int
    vizslas: int
    goldfish: int
    trees: int
    cars: int
    perfumes: int }

let optimal =
  { children = 3
    cats = 7
    samoyeds = 2
    pomeranians = 3
    akitas = 0
    vizslas = 0
    goldfish = 5
    trees = 3
    cars = 2
    perfumes = 1 }


let sues =
  readLines "./data.txt" |> Input.parseLines

let partOne =
  let matches attribute =
    match attribute with
    | Children x -> x = optimal.children
    | Cats x -> x = optimal.cats
    | Samoyeds x -> x = optimal.samoyeds
    | Pomeranians x -> x = optimal.pomeranians
    | Akitas x -> x = optimal.akitas
    | Vizslas x -> x = optimal.vizslas
    | Goldfish x -> x = optimal.goldfish
    | Trees x -> x = optimal.trees
    | Cars x -> x = optimal.cars
    | Perfumes x -> x = optimal.perfumes

  sues
  |> Array.find (fun (_, sue) ->
    let first, second, third = sue
    matches first && matches second && matches third)
  |> fst

let partTwo =
  let matches attribute =
    match attribute with
    | Children x -> x = optimal.children
    | Cats x -> x > optimal.cats
    | Samoyeds x -> x = optimal.samoyeds
    | Pomeranians x -> x < optimal.pomeranians
    | Akitas x -> x = optimal.akitas
    | Vizslas x -> x = optimal.vizslas
    | Goldfish x -> x < optimal.goldfish
    | Trees x -> x > optimal.trees
    | Cars x -> x = optimal.cars
    | Perfumes x -> x = optimal.perfumes

  sues
  |> Array.find (fun (_, sue) ->
    let first, second, third = sue
    matches first && matches second && matches third)
  |> fst

partOne |> printfn "Part I: %A"
partTwo |> printfn "Part II: %A"
