let readLines = System.IO.File.ReadAllLines

module Elevation =
  let getElevation ch =
    match ch with
    | 'S' -> 'a'
    | 'E' -> 'z'
    | x -> x
    |> (fun x -> int x - int 'a')

type Location = int * int
type ElevationMap = Map<Location, char * int>
type DistanceMap = Map<Location, int>

let isClimbable (elevations: ElevationMap) compare current neighbour =
  let getElevation x = elevations[x] |> snd

  if
    elevations.ContainsKey(neighbour)
    && elevations.ContainsKey(current)
  then
    compare (getElevation neighbour) (getElevation current)
  else
    false

let getClimbableOptions (m: ElevationMap) comparer (visited: Set<Location>) (x, y) =
  [ x, y - 1
    x - 1, y
    x + 1, y
    x, y + 1 ]
  |> List.filter (fun n ->
      (visited.Contains n = false) &&
      (match m.TryFind(x, y), m.TryFind n with
      | Some a, Some b -> comparer a b
      | _ -> false))
      
let updateDistances dist (distances: DistanceMap) (location: Location) =
  distances
  |> Map.change location (fun x ->
    match x with
    | Some v when v < dist -> Some v
    | Some _
    | None -> Some dist)

let climbHill (elevations: ElevationMap) start compare reachedEnd =
  let next =
    getClimbableOptions elevations compare

  let rec move (distances: DistanceMap) (visited: Set<Location>) =
    let position =
      distances
      |> Seq.reduce (fun curr dist ->
        if curr.Value < dist.Value then
          curr
        else
          dist)

    if reachedEnd position.Key then
      position.Value
    else
      let visited = visited.Add position.Key

      let distances =
        next visited position.Key
        |> Seq.fold (updateDistances (position.Value + 1)) (distances.Remove position.Key)

      move distances visited

  move (Map([ start, 0 ])) Set.empty

let buildData file =
  let graph =
    readLines file
    |> Array.map (fun line -> line |> Seq.indexed |> Array.ofSeq)
    |> Array.mapi (fun i line ->
      line
      |> Array.map (fun (j, ch) -> ((i, j), (ch, ch |> Elevation.getElevation))))
    |> Array.concat
    |> Map.ofArray

  let start =
    graph.Keys
    |> Seq.find (fun x -> graph[x] |> fst = 'S')

  let destination =
    graph.Keys
    |> Seq.find (fun x -> graph[x] |> fst = 'E')

  graph, start, destination

let partOne file =
  let graph, start, destination =
    buildData file

  let reachedDestination =
    fun x -> x = destination

  let compare =
    fun a b -> snd a - snd b <= 1

  climbHill graph start compare reachedDestination

let partTwo file =
  let graph, _, destination = buildData file

  let reachedDestination =
    fun x -> graph[x] |> fst = 'a'

  let compare =
    fun a b -> snd b >= snd a - 1

  climbHill graph destination compare reachedDestination

partOne "./test.txt" |> printfn "test part 1: %i"
partOne "./data.txt" |> printfn "real part 1: %i"

partTwo "./test.txt" |> printfn "test part 2: %i"
partTwo "./data.txt" |> printfn "real part 2: %i"
