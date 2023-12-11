open System
open System.Collections.Generic

let testInput =
  """5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526"""

type Cell = int

type State = Cell [] []

module Input =
  let parse (input: string) =
    input.Split "\n"
    |> Array.map (fun s -> s.ToCharArray() |> Array.map (string >> int))


module State =
  let private neighbours (state: State) (x, y) =
    [ (x - 1, y - 1)
      (x, y - 1)
      (x + 1, y - 1)
      (x - 1, y)
      (x + 1, y)
      (x - 1, y + 1)
      (x, y + 1)
      (x + 1, y + 1) ]
    |> List.filter (fun (x, y) ->
      match state |> Seq.tryItem x with
      | None -> false
      | Some row -> row |> Seq.tryItem y |> Option.isSome)

  let private getCellsOverNine (state: State) =
    state
    |> Seq.indexed
    |> Seq.fold
         (fun list (i, row) ->
           row
           |> Seq.indexed
           |> Seq.fold
                (fun list (j, cell) ->
                  match cell with
                  | x when x > 9 -> list @ [ (i, j) ]
                  | _ -> list)
                list)
         []

  let step (state: State) =
    let m, n =
      state.Length - 1, state[0].Length - 1

    let activated =
      [| for _ in 0..m -> [| for _ in 0..n -> false |] |]

    let flashQueue = Queue()

    // increase energy
    for i in 0..m do
      for j in 0..n do
        state[i][j] <- state[i][j] + 1

        if state[i][j] > 9 then
          flashQueue.Enqueue(i, j)
          activated[i][j] <- true

    while flashQueue.Count > 0 do
      let x, y = flashQueue.Dequeue()

      for i, j in neighbours state (x, y) do
        state[i][j] <- state[i][j] + 1

        if not (activated[i][j]) && state[i][j] > 9 then
          flashQueue.Enqueue(i, j)
          activated[i][j] <- true

    for i in { 0 .. state.Length - 1 } do
      for j in { 0 .. state[i].Length - 1 } do
        if state[i][j] > 9 then state[i][j] <- 0

    activated
    |> Seq.collect id
    |> Seq.filter id
    |> Seq.length

let input =
  System.IO.File.ReadAllText "./data.txt"

let partOne input =
  let data = input |> Input.parse

  [ 1..100 ]
  |> Seq.fold (fun count _ -> count + State.step data) 0


partOne testInput |> printfn "Part I (test): %i"
partOne input |> printfn "Part I: %i"

let partTwo input =
  let data = input |> Input.parse
  let all = data.Length * data[0].Length
  
  (+) 1
  |> Seq.initInfinite
  |> Seq.skipWhile(fun _ -> State.step data <> all)
  |> Seq.head

partTwo testInput |> printfn "Part II (test): %i"
partTwo input |> printfn "Part II: %i"