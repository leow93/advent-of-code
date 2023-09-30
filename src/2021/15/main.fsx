open System
open System.Collections.Generic

type RiskMap = Map<int * int, int>
type DistanceMap = Map<int * int, int>
type Visited = Set<int * int>
type Queue = PriorityQueue<int * int, int>

module Input =
  let testInput =
    """1163751742
      1381373672
      2136511328
      3694931569
      7463417111
      1319128137
      1359912421
      3125421639
      1293138521
      2311944581"""
      .Split("\n")
    |> Array.map (fun s -> s.Trim())

  let realInput =
    System.IO.File.ReadAllLines "./data.txt"

  let parse (lines: string []) : RiskMap =
    lines
    |> Array.mapi (fun i line ->
      line.ToCharArray()
      |> Array.mapi (fun j ch -> ((i, j), ch |> string |> int)))
    |> Array.concat
    |> Map

let findNeighbours (sizeX, sizeY) (x, y) =
  [ x - 1, y
    x, y - 1
    x + 1, y
    x, y + 1 ]
  |> List.filter (fun (x, y) -> 0 <= x && x < sizeX && 0 <= y && y < sizeY)

let enqueue x priority (q: Queue) =
  q.Enqueue(x, priority)
  q

let dequeue (q: Queue) = q.Dequeue()

let findLeastRisk (risks: RiskMap) =
  let size =
    risks.Count |> double |> sqrt |> int

  let start = (0, 0)
  let finish = (size - 1, size - 1)

  let neighbourFinder =
    findNeighbours (size, size)

  let seedQ = Queue() |> enqueue start 0

  let seedDistanceMap =
    risks
    |> Map.map (fun _ _ -> Int32.MaxValue)
    |> Map.add start 0

  let rec visit (dist: DistanceMap) (q: Queue) (visited: Visited) =
    match q.Count = 0 || dist[finish] < Int32.MaxValue with
    | true -> dist[finish]
    | false ->
      let location = q |> dequeue

      let updatedDM, updatedQ =
        neighbourFinder location
        |> List.filter (fun x -> not (visited.Contains x))
        |> Seq.map (fun n -> (n, dist[location] + risks[n]))
        |> Seq.filter (fun (n, d) -> d < dist[n])
        |> Seq.fold (fun (ud, uq) (n, d) -> ud |> Map.add n d, uq |> enqueue n d) (dist, q)

      visit updatedDM updatedQ (visited.Add location)

  visit seedDistanceMap seedQ Set.empty

let partOne input =
  let risks = input |> Input.parse

  findLeastRisk risks

Input.testInput
|> partOne
|> printfn "Part I (test): %i"

Input.realInput |> partOne |> printfn "Part I: %i"

let partTwo input =
  let risks = input |> Input.parse

  let size =
    risks.Count |> double |> sqrt |> int

  let risks' =
    Seq.allPairs [ 0..4 ] [ 0..4 ]
    |> Seq.allPairs risks
    |> Seq.map (fun (kvp, (i, j)) ->
      let x, y = kvp.Key
      let risk = (kvp.Value + i + j - 1) % 9 + 1

      (x + i * size, y + j * size), risk)
    |> Map

  findLeastRisk risks'

Input.testInput
|> partTwo
|> printfn "Part II (test): %i"

Input.realInput
|> partTwo
|> printfn "Part II: %i"
