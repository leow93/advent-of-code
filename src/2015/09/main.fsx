open System.Text.RegularExpressions

let readLines = System.IO.File.ReadAllLines
let split (sep: string) (s: string) = s.Split sep

type Route = (string * string) * int

let parseLines lines =
  let regex =
    "([\w\s?]+) to ([\w\s?]+) = (\d+)"

  lines
  |> Array.map (fun line ->
    let groups = Regex.Match(line, regex).Groups
    ((groups[1].Value, groups[2].Value), groups[3].Value |> int))

type DistanceMap = Map<string * string, int>

module DistanceMap =
  let build (routes: Route []) : DistanceMap = routes |> Map.ofArray

  let tryFind (a, b) map =
    let rec inner (a, b) map attempt =
      match map |> Map.tryFind (a, b) with
      | Some x -> Some x
      | None when attempt = 2 -> None
      | None -> inner (b, a) map (attempt + 1)

    inner (a, b) map 1


let buildDistanceMap (routes: Route []) = routes |> Map.ofArray

let buildPlacesSet (routes: Route []) =
  routes
  |> Array.fold
       (fun set route ->
         set
         |> Set.add (route |> fst |> fst)
         |> Set.add (route |> fst |> snd))
       Set.empty

let allPossibleRoutes places =
  (places, places)
  ||> Seq.allPairs
  |> Seq.filter (fun (a, b) -> a <> b)
  |> Array.ofSeq

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

let routeDistance distances (route: string list) =
  route
  |> List.pairwise
  |> List.fold
       (fun total (a, b) ->
         match distances |> DistanceMap.tryFind (a, b) with
         | Some d -> total + d
         | None -> total)
       0

let getAllPossibleDistances file =
  let routes = readLines file |> parseLines

  let distances = routes |> buildDistanceMap

  let places = routes |> buildPlacesSet

  let allPossibleRoutes =
    places |> List.ofSeq |> permute

  allPossibleRoutes
  |> List.fold
       (fun arr route ->
         [ routeDistance distances route ]
         |> List.append arr)
       List.empty



getAllPossibleDistances "./data.txt"
|> (fun xs ->
  printfn "Part I: %i" (xs |> List.min)
  printfn "Part II: %i" (xs |> List.max))
