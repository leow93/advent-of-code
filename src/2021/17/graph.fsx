open System.Collections.Generic

let adjacency n =
  let x = ResizeArray()
  for i in {0..n-1} do
    x.Insert(i, ResizeArray())
  x  

module Queue = 
  let enqueue s (q: Queue<'a>) =
    q.Enqueue s
    q
  let dequeue (q: Queue<'a>) = q.Dequeue


type Graph(n: int) =
  let mutable graph = List.init n (fun _ -> List.empty)
  member _.AddEdge(v, w) =
    graph <- 
      graph
      |> List.mapi(fun i xs ->
          if i = v then
            [w] |> List.append xs
          else
            xs
        )
  member _.BFS(s, fn) =
    let mutable visited = Set.ofList [s]
    let queue = Queue() |> Queue.enqueue s
    
    while queue.Count > 0 do
      let s = queue.Dequeue()
      fn s
      
      let xs = graph[s]
      for x in xs do
        if visited |> Set.contains x then
          ()
        else
          visited <- visited |> Set.add x
          queue.Enqueue x
          
          
let g =
  Graph(4)


g.AddEdge(0, 1);
g.AddEdge(0, 2);
g.AddEdge(1, 2);
g.AddEdge(2, 0);
g.AddEdge(2, 3);
g.AddEdge(3, 3)

printfn "BFS Starting from vertex 2"
g.BFS(2, printf "%i ")

printfn "BFS Starting from vertex 0"
g.BFS(0, printf "%i ")