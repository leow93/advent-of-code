open Microsoft.FSharp.Core

type Direction =
  | N
  | S
  | E
  | W

type Instruction =
  | L of int
  | R of int

let instructions =
  (System.IO.File.ReadAllText "./input.txt")
    .Split ", "
  |> Array.choose (fun x ->
    if x.StartsWith "R" then
      x.Substring(1) |> int |> R |> Some
    elif x.StartsWith "L" then
      x.Substring(1) |> int |> L |> Some
    else
      None)

let distance (x, y) = abs x + abs y

type State = (int * int) * Direction

module State =
  let initialState: State = ((0, 0), N)

  let moveDistanceInDirection ((x, y), direction) distance =
    match direction with
    | N -> ((x, y + distance), N)
    | S -> ((x, y - distance), S)
    | E -> ((x + distance, y), E)
    | W -> ((x - distance, y), W)

  let evolve ((x, y), direction) instruction =
    match direction, instruction with
    | N, L s
    | S, R s -> moveDistanceInDirection ((x, y), W) s
    | N, R s
    | S, L s -> moveDistanceInDirection ((x, y), E) s
    | W, L s
    | E, R s -> moveDistanceInDirection ((x, y), S) s
    | W, R s
    | E, L s -> moveDistanceInDirection ((x, y), N) s

  let partTwoCommands (_, direction) instruction =
    match direction, instruction with
    | N, L s
    | S, R s -> Array.create s W
    | N, R s
    | S, L s -> Array.create s E
    | W, L s
    | E, R s -> Array.create s S
    | W, R s
    | E, L s -> Array.create s N

let partOne =
  instructions
  |> Array.fold State.evolve State.initialState
  |> fst
  |> distance

partOne |> printfn "Part I: %i"

let partTwo =
  let rec inner visited (((x, y), d): State) (commands: Direction []) idx =
    match commands |> Array.tryItem idx with
    | None -> (visited, ((x, y), d), None)
    | Some dir ->
      let nextState =
        State.moveDistanceInDirection ((x, y), dir) 1

      let coord = nextState |> fst

      let nextVisited =
        visited
        |> Map.change coord (function
          | Some x -> Some(x + 1)
          | None -> Some 1)

      let visitedCount =
        nextVisited
        |> Map.tryFind coord
        |> Option.defaultValue 0

      if visitedCount = 2 then
        (nextVisited, nextState, Some coord)
      else
        inner nextVisited nextState commands (idx + 1)


  let rec loop visited state idx =
    match instructions |> Array.tryItem idx with
    | None -> None
    | Some instruction ->
      let commands =
        instruction |> State.partTwoCommands state

      match (inner visited state commands 0) with
      | _, _, Some x -> Some x
      | v, s, None -> loop v s (idx + 1)

  loop Map.empty State.initialState 0
  |> Option.map distance

partTwo |> printfn "Part II: %A"
