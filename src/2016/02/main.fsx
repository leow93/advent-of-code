type Move =
  | U
  | D
  | L
  | R

module Input =
  let private parseLine (line: string) =
    line.ToCharArray()
    |> Array.choose (function
      | 'U' -> Some U
      | 'D' -> Some D
      | 'L' -> Some L
      | 'R' -> Some R
      | _ -> None)
    |> List.ofArray

  let parse file =
    System.IO.File.ReadAllLines file
    |> Array.map parseLine
    |> List.ofArray

type State = string * string

module State =
  let initialState = "5", ""

  let private foldMovePartOne position move =
    let number = int position
    match number, move with
    | x, U when x >= 4 -> (int x - 3)
    | x, U -> x
    | x, D when x <= 6 -> (x + 3)
    | x, D -> x
    | x, L when x <> 1 && x <> 4 && x <> 7 -> x - 1
    | x, L -> x
    | x, R when x <> 3 && x <> 6 && x <> 9 -> x + 1
    | x, R -> x
    |> string
  
  let private foldMovePartTwo position move =
    match position, move with
    | "1", D -> "3"
    | "1", _ -> "1"
    | "2", D -> "6"
    | "2", R -> "3"
    | "2", _ -> "2"
    | "3", U -> "1"
    | "3", L -> "2"
    | "3", R -> "4"
    | "3", D -> "7"
    | "4", D -> "8"
    | "4", L -> "3"
    | "4", _ -> "4"
    | "5", R -> "6"
    | "5", _ -> "5"
    | "6", L -> "5"
    | "6", D -> "A"
    | "6", R -> "7"
    | "6", U -> "2"
    | "7", L -> "6"
    | "7", D -> "B"
    | "7", R -> "8"
    | "7", U -> "3"
    | "8", L -> "7"
    | "8", D -> "C"
    | "8", R -> "9"
    | "8", U -> "4"
    | "9", L -> "8"
    | "9", _ -> "9"
    | "A", U -> "6"
    | "A", R -> "B"
    | "A", _ -> "A"
    | "B", L -> "A"
    | "B", D -> "D"
    | "B", R -> "C"
    | "B", U -> "7"
    | "C", U -> "8"
    | "C", L -> "B"
    | "C", _ -> "C"
    | "D", U -> "B"
    | "D", _ -> "D"
    | x, _ -> x
    
  let nextNumberPartOne current line = line |> List.fold foldMovePartOne current
  let nextNumberPartTwo current line = line |> List.fold foldMovePartTwo current

  let foldLine fn (position, code) line =
    let next = fn position line
    (next, code + string next)

let partOne file =
  let moves = file |> Input.parse

  moves
  |> List.fold (State.foldLine State.nextNumberPartOne) State.initialState
  |> snd

partOne "./test.txt"
|> printfn "Part I (test): %s"

partOne "./input.txt"
|> printfn "Part I (real): %s"

let partTwo file =
  let moves = file |> Input.parse

  moves
  |> List.fold (State.foldLine State.nextNumberPartTwo) State.initialState
  |> snd

partTwo "./test.txt"
|> printfn "Part II (test): %s"

partTwo "./input.txt"
|> printfn "Part II (real): %s"