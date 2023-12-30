#load "../../utils/Utils.fsx"

open System.Collections.Generic
open System.Diagnostics
open Utils

let input = Input.readLines ()

type Cell =
  | Path
  | Forest
  | SlopeUp
  | SlopeDown
  | SlopeLeft
  | SlopeRight

let get (x, y) grid = grid |> Array.item x |> Array.item y

module Parser =
  let parse (lines: string[]) =
    lines
    |> Array.map (fun line ->
      line.ToCharArray()
      |> Array.map (function
        | '.' -> Path
        | '#' -> Forest
        | '>' -> SlopeRight
        | '<' -> SlopeLeft
        | '^' -> SlopeUp
        | 'v' -> SlopeDown
        | _ -> failwith "Unknown cell"))

module PathFinder =
  let findStart grid =
    0, grid |> Array.head |> Array.findIndex (fun cell -> cell = Path)

  let findEnd grid =
    Array.length grid - 1, grid |> Array.last |> Array.findIndex (fun cell -> cell = Path)

  let exists grid (x, y) =
    x >= 0 && x < Array.length grid && y >= 0 && y < Array.length grid[0]


  let getCrossings adjacent start finish grid =
    let rows = Array.length grid
    let cols = Array.length grid[0]

    seq {
      for r in 0 .. rows - 1 do
        for c in 0 .. cols - 1 do
          if grid |> get (r, c) <> Forest && List.length (adjacent (r, c)) > 2 then
            r, c
    }
    |> Set.ofSeq
    |> Set.add start
    |> Set.add finish


  let distanceToNeighbors adjacent crossings source =
    let distances = Dictionary<int * int, int>()
    let queue = Queue<Set<int * int> * (int * int)>()
    queue.Enqueue(Set.empty, source)

    let rec visit () =
      match queue.TryDequeue() with
      | false, _ -> distances
      | _, (path, point) when path.Contains(point) -> visit ()
      | _, (path, point) when point <> source && Set.contains point crossings ->
        distances[point] <- Set.count path
        visit ()
      | _, (path, point) ->
        adjacent point |> Seq.iter (fun next -> queue.Enqueue(Set.add point path, next))

        visit ()

    visit ()

  let dfs getNeighbours start finish cost =
    let sw = Stopwatch()
    sw.Start()

    let q = Queue<Set<int * int> * int * (int * int)>()
    q.Enqueue(Set.empty, 0, start)
    let mutable longestPath = 0

    let printEveryNSeconds n =
      async {
        while sw.IsRunning do
          let! _ = Async.Sleep(1000* n)
          printfn "Elapsed ms: %A" sw.ElapsedMilliseconds
          printfn "Longest path: %A" longestPath
          printfn "q length: %A" q.Count

      }
      
    printEveryNSeconds 2 |> Async.Start

    let rec visit () =
      match q.TryDequeue() with
      | false, _ ->
        sw.Stop()
        printfn "Elapsed: %A" sw.Elapsed
        longestPath
      | true, (_, dist, point) when point = finish ->
        longestPath <- max longestPath dist
        visit ()
      | true, (path, _, point) when path.Contains point -> visit ()
      | true, (path, dist, point) ->
        let neighbours = getNeighbours point

        neighbours
        |> List.iter (fun next -> q.Enqueue(Set.add point path, dist + cost point next, next))

        visit ()

    visit ()

let grid = input |> Parser.parse
let rows = Array.length grid
let cols = Array.length grid[0]

let partOne grid =
  let start = PathFinder.findStart grid
  let finish = PathFinder.findEnd grid

  let getNextCells (x, y) =
    let up, down, left, right = (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)
    let curr = grid |> get (x, y)
    let all = [ up; down; left; right ]

    match curr with
    | SlopeUp -> [ up ]
    | SlopeDown -> [ down ]
    | SlopeLeft -> [ left ]
    | SlopeRight -> [ right ]
    | _ ->
      all
      |> List.filter (PathFinder.exists grid)
      |> List.choose (fun (x1, y1) ->
        match curr with
        | Forest -> None
        | SlopeRight when y1 < y -> None
        | SlopeLeft when y1 > y -> None
        | SlopeUp when x1 < x -> None
        | SlopeDown when x1 > x -> None
        | _ -> Some(x1, y1))

  PathFinder.dfs getNextCells start finish (fun _ _ -> 1)

let partTwo grid =
  let start = PathFinder.findStart grid
  let finish = PathFinder.findEnd grid

  let adjacent (r, c) =
    [ r - 1, c; r + 1, c; r, c - 1; r, c + 1 ]
    |> List.filter (PathFinder.exists grid)
    |> List.filter (fun (r, c) -> grid[r][c] <> Forest)

  let crossings =
    seq {
      for r in 0 .. rows - 1 do
        for c in 0 .. cols - 1 do
          if grid |> get (r, c) <> Forest && List.length (adjacent (r, c)) > 2 then
            yield r, c
    }
    |> Set.ofSeq
    |> Set.add start
    |> Set.add finish

  printfn "n crossings %A" crossings.Count

  let crossings = PathFinder.getCrossings adjacent start finish grid

  let distanceToNeighbours source =
    let distances = Dictionary<int * int, int>()
    let queue = Queue<Set<int * int> * (int * int)>()
    queue.Enqueue(Set.empty, source)

    let rec visit () =
      match queue.TryDequeue() with
      | false, _ -> distances
      | _, (path, pt) when path.Contains(pt) -> visit ()
      | _, (path, pt) when pt <> source && Set.contains pt crossings ->
        distances[pt] <- Set.count path
        visit ()
      | _, (path, pt) ->
        adjacent pt |> Seq.iter (fun next -> queue.Enqueue(Set.add pt path, next))
        visit ()

    visit ()

  let distances =
    crossings |> Seq.map (fun src -> src, distanceToNeighbours src) |> dict

  let adjacentCrossings pt = distances[pt].Keys |> Seq.toList

  PathFinder.dfs adjacentCrossings start finish (fun src dest -> distances[src][dest])

grid |> partOne |> printfn "Part one: %A"
grid |> partTwo |> printfn "Part two: %A"
