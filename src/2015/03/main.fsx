let readFile = System.IO.File.ReadAllText

type Direction =
  | North
  | South
  | East
  | West

module Directions =
  let parse (ch: char) =
    match ch with
    | '^' -> Some North
    | 'v' -> Some South
    | '>' -> Some East
    | '<' -> Some West
    | _ -> None

  let private getNextPosition (x, y) direction =
    match direction with
    | North -> (x, y + 1)
    | South -> (x, y - 1)
    | East -> (x + 1, y)
    | West -> (x - 1, y)

  let move (map: Map<int * int, bool>) (position: int * int) direction =
    let next =
      getNextPosition position direction

    map |> Map.change next (fun _ -> Some true),
    next

let partOne file =
  readFile file
  |> (fun s -> s.ToCharArray())
  |> Array.choose Directions.parse
  |> Array.fold (fun (map, position) -> Directions.move map position) (Map.empty, (0, 0))
  |> fst
  |> Map.values
  |> Seq.sumBy (fun x ->
    match x with
    | true -> 1
    | false -> 0)

partOne "./data.txt" |> printfn "Part I: %i"

let partTwo file =
  let directions =
    readFile file
    |> (fun s -> s.ToCharArray())
    |> Array.choose Directions.parse

  let santasDirections, roboSantasDirections =
    directions
    |> Array.indexed
    |> Array.fold
         (fun (santas, roboSantas) (i, direction) ->
           if i % 2 = 0 then
             (santas, [| direction |] |> Array.append roboSantas)
           else
             ([| direction |] |> Array.append santas, roboSantas))
         (Array.empty, Array.empty)

  let map =
    santasDirections
    |> Array.fold (fun (map, position) -> Directions.move map position) (Map.empty, (0, 0))
    |> fst

  roboSantasDirections
  |> Array.fold (fun (map, position) -> Directions.move map position) (map, (0, 0))
  |> fst
  |> Map.values
  |> Seq.sumBy (fun x ->
    match x with
    | true -> 1
    | false -> 0)

partTwo "./data.txt" |> printfn "Part II: %i"
