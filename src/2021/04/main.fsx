open System

type Cell = int * bool
type Grid = Cell [] []

let tupleMap fn y x = (fn x, y)

let readInput file =
  match System.IO.File.ReadAllLines file |> List.ofArray with
  | head :: tail ->
    let drawnNumbers =
      head.Split "," |> Array.map int

    let boards =
      tail
      |> List.filter (not << String.IsNullOrWhiteSpace)
      |> List.chunkBySize 5
      |> List.map (fun grid ->
        grid
        |> List.map (fun line ->
          line.Split([| ' ' |])
          |> Array.choose (fun x ->
            match Int32.TryParse x with
            | true, x -> Some(x, false)
            | _ -> None)
          |> List.ofArray))

    drawnNumbers, boards
  | _ -> failwithf "Bad input"

module Bingo =
  let evolve board number =
    board
    |> List.map (fun row ->
      row
      |> List.map (fun cell ->
        match snd cell with
        | true -> cell
        | false -> (fst cell, fst cell = number)))


let isWinningBoard board =
  if (board
      |> List.exists (fun row -> row |> List.forall (fun cell -> cell |> snd))) then
    true
  else
    let transposed = board |> List.transpose

    transposed
    |> List.exists (fun row -> row |> List.forall (fun cell -> cell |> snd))

let checkWinner boards =
  boards |> List.tryFindIndex isWinningBoard

let calculateScore board numberDrawn =
  (board
   |> List.fold
        (fun total row ->
          row
          |> List.fold
               (fun sum cell ->
                 if snd cell then
                   sum
                 else
                   sum + (fst cell))
               total)
        0)
  * numberDrawn

let findAllWinners boards =
  boards
  |> List.indexed
  |> List.fold
       (fun set (i, board) ->
         if isWinningBoard board then
           set |> Set.add i
         else
           set)
       Set.empty
  |> List.ofSeq

let partOne file =
  let numbers, boards = readInput file

  let rec loop boards idx =
    match numbers |> Array.tryItem idx with
    | Some n ->
      let nextBoards =
        boards
        |> List.map (fun board -> Bingo.evolve board n)

      match checkWinner nextBoards with
      | Some x -> calculateScore nextBoards[x] n
      | None -> loop nextBoards (idx + 1)
    | None -> failwithf "no result"

  loop boards 0

printfn "Part I: %i" (partOne "./data.txt")

let addToSet set xs =
  xs |> Seq.fold (fun set x -> set |> Set.add x) set

let partTwo file =
  let numbers, boards = readInput file

  let rec loop boards winners idx =
    match numbers |> Array.tryItem idx with
    | Some n ->
      let nextBoards =
        boards
        |> List.map (fun board -> Bingo.evolve board n)

      let winners' =
        (findAllWinners nextBoards)
        |> List.append winners
        |> List.distinct

      if winners'.Length = nextBoards.Length then
        let lastWinner = winners' |> List.last
        calculateScore (nextBoards[lastWinner]) n
      else
        loop nextBoards winners' (idx + 1)
    | None -> failwithf "no result"

  loop boards [] 0

printfn "Part II: %i" (partTwo "./data.txt")
