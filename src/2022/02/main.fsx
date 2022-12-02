let readLines = System.IO.File.ReadLines
let split (separator: string) (value: string) = value.Split(separator)

type Play =
  | Rock
  | Paper
  | Scissors

module Play =
  let deserialize s =
    match s with
    | "A"
    | "X" -> Rock
    | "B"
    | "Y" -> Paper
    | "C"
    | "Z" -> Scissors
    | _ -> failwith $"unknown play: {s}"
    
  let score plays =
    match plays with
    | Rock, Rock -> 3 + 1
    | Rock, Paper -> 6 + 2
    | Rock, Scissors -> 0 + 3
    | Paper, Rock -> 0 + 1
    | Paper, Paper -> 3 + 2
    | Paper, Scissors -> 6 + 3
    | Scissors, Rock -> 6 + 1
    | Scissors, Paper -> 0 + 2
    | Scissors, Scissors -> 3 + 3    

type Result = Win | Draw | Lose
module Result =
  let deserialize s =
    match s with
    | "X" -> Lose
    | "Y" -> Draw
    | "Z" -> Win
    | _ -> failwith $"unknown result: {s}"
    
  let getMoveForResult play result =
    match play, result with
    | Rock, Win -> Paper
    | Rock, Draw -> Rock
    | Rock, Lose -> Scissors
    | Paper, Win -> Scissors
    | Paper, Draw -> Paper
    | Paper, Lose -> Rock
    | Scissors, Win -> Rock
    | Scissors, Draw -> Scissors
    | Scissors, Lose -> Paper

let partOne file =
  readLines file
  |> Seq.map (
    split " "
    >> fun x -> (Play.deserialize x[0], Play.deserialize x[1])
  )
  |> Seq.sumBy Play.score
  
let partTwo file =
  readLines file
  |> Seq.map(split " " >> fun x ->
      let play = Play.deserialize x[0]
      play, Result.getMoveForResult play (Result.deserialize x[1])
    )
  |> Seq.sumBy Play.score


printfn "Test"
partOne "./test.txt" |> printf "Part I: %A\n"
partTwo "./test.txt" |> printf "Part II: %A\n"

printfn "\nActual"
partOne "./data.txt" |> printf "Part I: %A\n"
partTwo "./data.txt" |> printf "Part II: %A\n"