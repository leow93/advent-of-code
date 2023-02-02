open System.Collections.Generic
open System.Text.RegularExpressions

type Ingredient =
  { name: string
    capacity: int
    durability: int
    flavour: int
    texture: int
    calories: int }

module Input =
  let private regex =
    "(\w+): capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)"

  let private parseLine (line: string) =
    Regex.Match(line, regex).Groups
    |> (fun groups ->
      let name = groups[1].Value

      name,
      { name = name
        capacity = groups[2].Value |> int
        durability = groups[3].Value |> int
        flavour = groups[4].Value |> int
        texture = groups[5].Value |> int
        calories = groups[6].Value |> int })

  let parseLines lines = lines |> Array.map parseLine
  let readLines = System.IO.File.ReadAllLines

let update k v map =
  map
  |> Map.change k (function
    | None -> Some v
    | Some x -> Some(v + x))

let getIngredientProp ingredient prop =
  match prop with
  | "capacity" -> ingredient.capacity
  | "durability" -> ingredient.durability
  | "flavour" -> ingredient.flavour
  | "texture" -> ingredient.texture
  | _ -> failwithf "unexpected prop %s" prop

let updateDict k fn (dict: Dictionary<_, _>) =
  match dict.TryGetValue k with
  | true, x -> dict[k] <- fn (Some x)
  | false, _ -> dict[k] <- fn None

let makeCookie (ingredients: Map<string, Ingredient>) (teaspoons: Map<string, int>) ignoreCaloriesFn =
  let map = Dictionary<_, _>()

  let ingredientsSet =
    ingredients |> Map.keys |> Set.ofSeq

  for ingredient in ingredientsSet do
    match ingredients |> Map.tryFind ingredient, teaspoons |> Map.tryFind ingredient with
    | Some attributes, Some teaspoonsCount ->
      updateDict
        "calories"
        (function
        | None -> attributes.calories * teaspoonsCount
        | Some x -> x + (attributes.calories * teaspoonsCount))
        map

      updateDict
        "capacity"
        (function
        | None -> attributes.capacity * teaspoonsCount
        | Some x -> x + (attributes.capacity * teaspoonsCount))
        map

      updateDict
        "durability"
        (function
        | None -> attributes.durability * teaspoonsCount
        | Some x -> x + (attributes.durability * teaspoonsCount))
        map

      updateDict
        "flavour"
        (function
        | None -> attributes.flavour * teaspoonsCount
        | Some x -> x + (attributes.flavour * teaspoonsCount))
        map

      updateDict
        "texture"
        (function
        | None -> attributes.texture * teaspoonsCount
        | Some x -> x + (attributes.texture * teaspoonsCount))
        map
    | _ -> failwithf "not found"

  let calories = map["calories"]
  map.Remove "calories" |> ignore

  if ignoreCaloriesFn calories then
    0
  else
    let values = map.Values
    if values |> Seq.exists (fun x -> x <= 0) then
      0
    else
      values |> Seq.reduce (*)

let main file filter =
  let ingredients =
    Input.readLines file
    |> Input.parseLines
    |> Map.ofArray

  let mutable allPossibleCookies = []

  for frosting in { 0..100 } do
    let candyMax = 100 - frosting

    for candy in { 0..candyMax } do
      let butterScotchMax = 100 - frosting - candy

      for butterScotch in { 0..butterScotchMax } do

        let sugar =
          100 - frosting - candy - butterScotch

        let teaspoons =
          [ ("Frosting", frosting)
            ("Candy", candy)
            ("Butterscotch", butterScotch)
            ("Sugar", sugar) ]
          |> Map.ofList

        allPossibleCookies <-
          [ makeCookie ingredients teaspoons filter ]
          |> List.append allPossibleCookies

  allPossibleCookies
  |> List.sortDescending
  |> List.head

let partOne file = main file (fun _ -> false)
let partTwo file = main file (fun x -> x <> 500)

partOne "./data.txt" |> printfn "Part I: %i"
partTwo "./data.txt" |> printfn "Part II: %i"
