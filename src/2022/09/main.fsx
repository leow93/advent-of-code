let readLines = System.IO.File.ReadAllLines
let split (separator: string) (s: string) = s.Split(separator)
type Coordinates = { x: int; y: int }
let origin = { x = 0; y = 0 }

type Move =
  | Up
  | Down
  | Left
  | Right

module Move =
  let parseLine (s: string) =
    match split " " s with
    | [| "U"; x |] -> Up |> Array.create (int x)
    | [| "D"; x |] -> Down |> Array.create (int x)
    | [| "L"; x |] -> Left |> Array.create (int x)
    | [| "R"; x |] -> Right |> Array.create (int x)
    | _ -> failwithf "unparseable line: %s" s

module State =
  type State =
    { H: Coordinates
      T: Coordinates
      TailVisits: Set<Coordinates> }

  let initial =
    { H = origin
      T = origin
      TailVisits = Set.ofList [ origin ] }

  let moveHead move head =
    match move with
    | Up -> { head with y = head.y + 1 }
    | Down -> { head with y = head.y - 1 }
    | Right -> { head with x = head.x + 1 }
    | Left -> { head with x = head.x - 1 }

  let moveTail head tail =
    let yDelta = head.y - tail.y
    let xDelta = head.x - tail.x

    if abs xDelta <= 1 && abs yDelta <= 1 then
      tail
    else if xDelta = 0 && abs yDelta > 1 then
      let step = abs yDelta / yDelta
      { tail with y = tail.y + step }
    else if yDelta = 0 && abs xDelta > 1 then
      let step = abs xDelta / xDelta
      { tail with x = tail.x + step }
    else
      { x = tail.x + (abs xDelta / xDelta)
        y = tail.y + (abs yDelta / yDelta) }

  let evolve state move =
    let head = moveHead move state.H
    let tail = moveTail head state.T

    { H = head
      T = tail
      TailVisits = state.TailVisits |> Set.add tail }

  let countVisits state = state.TailVisits |> Set.count

module State2 =
  type State =
    { Parts: Coordinates array
      TailVisits: Set<Coordinates> }

  let initial =
    { Parts = Array.create 10 origin
      TailVisits = Set.ofList [ origin ] }

  let evolve state move =
    let head =
      state.Parts |> Array.head |> State.moveHead move

    let rest =
      (state.Parts |> Array.skip 1)
      |> Array.append [| head |]
      |> Array.pairwise
      |> Array.map (fun (head, tail) -> State.moveTail head tail)

    { Parts = rest |> Array.append [| head |]
      TailVisits = state.TailVisits |> Set.add (Array.last rest) }

let partOne file =
  readLines file
  |> Array.map Move.parseLine
  |> Array.concat
  |> Array.fold State.evolve State.initial
  |> State.countVisits

let partTwo file =
  readLines file
  |> Array.map Move.parseLine
  |> Array.concat
  |> Array.fold State2.evolve State2.initial
  |> (fun state -> state.TailVisits |> Set.count)

partOne "./test.txt"
|> printfn "test part one: %i"

partTwo "./test.txt"
|> printfn "test part two: %i"

partOne "./data.txt"
|> printfn "actual part one: %i"

partTwo "./data.txt"
|> printfn "actual part two: %i"
