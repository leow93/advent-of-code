open System.Collections.Generic

let readLines = System.IO.File.ReadLines

type ValveId = string

type TunnelsMap = Map<ValveId, Set<ValveId>>
type PressureMap = Map<ValveId, int>

let splitByChars (chars: char list) (str: string) = str.Split(chars |> Array.ofList)

let parsePressuresAndTunnels file : PressureMap * TunnelsMap =
  readLines file
  |> Array.ofSeq
  |> Array.map (
    splitByChars [ ' '; ','; '='; ';' ]
    >> fun arr -> arr |> Array.filter (fun x -> x <> "")
    >> fun arr -> (arr[1], int arr[5]), (arr[1], Array.skip 10 arr |> Set.ofArray)
  )
  |> fun arr -> Array.map fst arr |> Map.ofArray, Array.map snd arr |> Map.ofArray


(*
Breadth_First_Serach( Graph, X ) // Here, Graph is the graph that we already have and X is the source node

Let Q be the queue
Q.enqueue( X ) // Inserting source node X into the queue
Mark X node as visited.

While ( Q is not empty )
Y = Q.dequeue( ) // Removing the front node from the queue

Process all the neighbors of Y, For all the neighbors Z of Y
If Z is not visited, Q. enqueue( Z ) // Stores Z in Q
Mark Z as visited
*)

let memoize f =
  let cache = Dictionary<_, _>()

  fun x ->
    match cache.TryGetValue x with
    | true, value -> value
    | false, _ ->
      let value = f x
      cache.Add(x, value)
      value

let bfs (graph: TunnelsMap) (start: string) (dest: string) =
  let visited = Set([ start ])
  let queue = Queue([ start, 0 ])

  let rec loop () =
    match queue.TryDequeue() with
    | false, _ -> failwithf "empty"
    | true, (position, distance) ->
      if position = dest then
        distance
      else
        visited.Add position |> ignore

        graph[position]
        |> Set.filter (visited.Contains >> not)
        |> Seq.iter (fun next -> queue.Enqueue(next, distance + 1))

        loop ()

  loop ()

let tryMax seq =
  match seq |> Seq.isEmpty with
  | true -> 0
  | _ -> seq |> Seq.max


let cache = Dictionary<_, _>()

let bfsMemo = memoize bfs

let rec maxPressure (pressures: PressureMap) (tunnels: TunnelsMap) (valves: Set<ValveId>) current time =
  let inner () =
    (pressures[current] * time)
    + (valves
       |> Seq.map (fun next -> next, bfsMemo tunnels current next + 1)
       |> Seq.filter (fun (_, timeSpent) -> timeSpent < time)
       |> Seq.map (fun (next, timeSpent) -> maxPressure pressures tunnels (Set.remove next valves) next (time - timeSpent))
       |> tryMax)

  let key = (current, time, valves)

  match cache.TryGetValue key with
  | true, value -> value
  | false, _ ->
    let value = inner ()
    cache.Add(key, value)
    value

let partOne (pressures, tunnels) =
  let valves =
    pressures
    |> Map.filter (fun _ v -> v > 0)
    |> Map.keys
    |> Set.ofSeq

  maxPressure pressures tunnels valves "AA" 30

let testData =
  parsePressuresAndTunnels "./test.txt"

let realData =
  parsePressuresAndTunnels "./data.txt"

testData |> partOne |> printfn "Part I (test): %A"
realData |> partOne |> printfn "Part I (real): %A"
