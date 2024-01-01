#load "../../utils/Utils.fsx"

open System
open System.Collections.Generic
open Utils

let input = Input.readLines ()

module GraphViz =
  let toGraphVizCmd (input: string[]) =
    let mutable result = "graph G { \n"

    for line in input do
      match line.Split([| ':' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray with
      | [ parent; children ] ->
        let xs = String.Join(",", Strings.split " " (children.Trim()))
        result <- result + $"{parent}--{xs}\n"
      | _ -> ()

    result + "}"
  
let buildGraph (input: string[]) =
  let mutable result = Map.empty

  let append (key: string) (value: string) map =
    match map |> Map.tryFind (key) with
    | Some xs -> map.Add(key, value :: xs)
    | None -> map.Add(key, [ value ])

  for line in input do
    match line.Split([| ':' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray with
    | [ parent; children ] ->
      let children =
        children.Trim().Split([| ' ' |], StringSplitOptions.RemoveEmptyEntries)
        |> List.ofArray

      for child in children do
        result <- append child parent result
        result <- append parent child result
    | _ -> ()

  result

let graph = buildGraph input

let removeEdge (from: string) (to_: string) graph =
  let remove (key: string) (value: string) map =
    match map |> Map.tryFind key with
    | Some xs -> map.Add(key, List.filter ((<>) value) xs)
    | None -> map

  graph |> remove from to_ |> remove to_ from

let removeEdges edges graph =
  edges |> List.fold (fun acc (from, to_) -> removeEdge from to_ acc) graph

// cuts to make: fbd-lzd, fxn-pqd, kcn-szl

let bfs graph start =
  let visited = HashSet()
  let queue = Queue()
  queue.Enqueue(start)
  visited.Add(start) |> ignore

  while queue.Count > 0 do
    let node = queue.Dequeue()
    
    if start = "fbd" && node = "lzd" then
      printfn "found lzd"

    match graph |> Map.tryFind node with
    | Some xs ->
      for x in xs do
        if not (visited.Contains(x)) then
          queue.Enqueue(x)
          visited.Add(x) |> ignore
    | None -> ()

  visited.Count

// let cuts = [ ("hfx", "pzl"); ("bvb", "cmg"); ("nvd", "jqt") ]
let cuts = [ ("fbd", "lzd"); ("fxn", "ptq"); ("kcn", "szl") ]

printfn "graph[fbd] = %A" (graph |> Map.find "fbd")

let graph' = graph |> removeEdges cuts

printfn "graph'[fbd] = %A" (graph' |> Map.find "fbd")
printfn "fbd count %d" (bfs graph' "fbd") 
printfn "lzd count %d" (bfs graph' "lzd") 

bfs graph' "fbd" * bfs graph' "lzd" |> printfn "%d"

// 2292196 too high
