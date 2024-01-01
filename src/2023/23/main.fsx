#load "../../utils/Utils.fsx"

open Utils
open System.Collections.Generic

let input = Input.readLines ()

let height, width = input.Length, input[0].Length

let start =
  [ 0 .. width - 1 ]
  |> Seq.map (fun col -> 0, col)
  |> Seq.find (fun (row, col) -> input[row][col] = '.')

let target =
  [ 0 .. width - 1 ]
  |> Seq.map (fun col -> height - 1, col)
  |> Seq.find (fun (row, col) -> input[row][col] = '.')

let inbounds (r, c) =
  r >= 0 && c >= 0 && r < height && c < width

let notForest (r, c) = input[r][c] <> '#'

let validPoint p = inbounds p && notForest p

let adjacentWithSlopes (i, j) =
  match input[i][j] with
  | '>' -> [ i, j + 1 ]
  | '<' -> [ i, j - 1 ]
  | 'v' -> [ i + 1, j ]
  | '^' -> [ i - 1, j ]
  | '.' -> [ i - 1, j; i + 1, j; i, j - 1; i, j + 1 ]
  | _ -> failwith "error"
  |> List.filter validPoint

let bfsLongestPath adjacency cost source target =
  let queue = Queue<Set<int * int> * int * (int * int)>()
  queue.Enqueue(Set.empty, 0, source)
  let mutable longestPathLength = 0

  let rec visit () =
    match queue.TryDequeue() with
    | false, _ -> longestPathLength
    | _, (_, dist, pt) when pt = target ->
      longestPathLength <- max longestPathLength dist
      visit ()
    | _, (path, _, pt) when path.Contains(pt) -> visit ()
    | _, (path, dist, pt) ->
      adjacency pt
      |> Seq.iter (fun next -> queue.Enqueue(Set.add pt path, dist + cost pt next, next))

      visit ()

  visit ()

let part1 = bfsLongestPath adjacentWithSlopes (fun _ _ -> 1) start target
printfn "Part one: %d" part1

let adjacent (i, j) =
  [ i - 1, j; i + 1, j; i, j - 1; i, j + 1 ] |> List.filter validPoint

let crossings =
  seq {
    for i in 0 .. height - 1 do
      for j in 0 .. width - 1 do
        if input[i][j] <> '#' && List.length (adjacent (i, j)) > 2 then
          i, j
  }
  |> Set.ofSeq
  |> Set.add start
  |> Set.add target

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

// Dictionary<source, Dictionary<target, distance>>
let distances =
  crossings |> Seq.map (fun src -> src, distanceToNeighbours src) |> dict

let adjacentCrossings pt = distances[pt].Keys |> seq

let part2 =
  bfsLongestPath adjacentCrossings (fun src dest -> distances[src][dest]) start target

printfn "Part two: %d" part2
