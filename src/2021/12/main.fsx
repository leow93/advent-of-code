type Cave =
  | Start
  | End
  | Small of string
  | Big of string

type Tunnel = Cave * Cave

type TunnelMap = Map<Cave, Cave []>

module Input =
  let testInput =
    """start-A
    start-b
    A-c
    A-b
    b-d
    A-end
    b-end"""
      .Split "\n"

  let realInput =
    System.IO.File.ReadAllLines "./data.txt"

  let private parseLine (line: string) =
    line.Split "-"
    |> Array.map (function
      | "start" -> Start
      | "end" -> End
      | x when x.ToLower() = x -> Small x
      | x -> Big x)
    |> function
      | [| a; b |] -> (a, b)
      | _ -> failwithf "Invalid input"

  let private toTunnelMap (tunnels: Tunnel []) : TunnelMap =
    tunnels
    |> Seq.collect (fun (a, b) -> [ (a, b); (b, a) ])
    |> Seq.groupBy fst
    |> Seq.map (fun (a, edges) -> (a, edges |> Seq.map snd |> Seq.toArray))
    |> Map

  let parse (lines: string []) =
    lines
    |> Array.map (fun s -> s.Trim() |> parseLine)
    |> toTunnelMap

let rec findPaths (visited: Set<Cave>) (map: TunnelMap) allowTwice (current: Cave) =
  let nextPaths visited allowTwice =
    map[current]
    |> Seq.filter (fun next ->
      not (visited |> Set.contains next))
    |> Seq.collect (findPaths visited map allowTwice)
    |> Seq.toList

  let pathsFromCurrent =
    match current with
    | End -> [ [ current ] ]
    | Small _ ->
      let nextVisited = visited.Add current
      let mutable pathsFromCurrent =
        nextPaths nextVisited allowTwice
        
      if allowTwice then
        pathsFromCurrent <-
          pathsFromCurrent
          |> Seq.append(nextPaths visited false)
          |> Set
          |> Seq.toList
      pathsFromCurrent
    | _ -> nextPaths visited allowTwice

  let pathsWithCurrent =
    pathsFromCurrent
    |> List.map (fun pathToEnd -> current :: pathToEnd)

  pathsWithCurrent

let countPaths allowTwice (map: TunnelMap) =
  let visited = Set<Cave>([ Start ])
  Start |> findPaths visited map allowTwice |> Seq.length

let partOne input =
  input |> Input.parse |> countPaths false

let partTwo input =
  input |> Input.parse |> countPaths true  

Input.testInput
|> partOne
|> printfn "Part I (test): %A"

Input.realInput
|> partOne
|> printfn "Part I: %A"


Input.testInput
|> partTwo
|> printfn "Part II (test): %A"

Input.realInput
|> partTwo
|> printfn "Part II: %A"
