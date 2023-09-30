open System

type Coord = int * int

type Cell =
  | On
  | Off

type Fold =
  | Up of int
  | Left of int


module Input =
  let parse (lines: string []) =
    let coords =
      lines
      |> Array.takeWhile (fun line -> line.Contains ",")
      |> Array.choose (fun line ->
        match line.Split "," with
        | [| a; b |] -> Some(int a, int b)
        | _ -> None)

    let instructions =
      lines
      |> Array.filter (fun line -> line.Contains "fold")
      |> Array.choose (fun line ->
        match line.Split "=" with
        | [| axis; value |] when axis.EndsWith "x" -> value |> int |> Left |> Some
        | [| axis; value |] when axis.EndsWith "y" -> value |> int |> Up |> Some
        | _ -> None)

    coords, instructions

  let testInput =
    """6,10
    0,14
    9,10
    0,3
    10,4
    4,11
    6,0
    6,12
    4,1
    0,13
    10,12
    3,4
    3,0
    8,4
    1,10
    2,14
    8,10
    9,0

    fold along y=7
    fold along x=5"""
      .Split "\n"
    |> Array.map (fun s -> s.Trim())

  let realInput =
    System.IO.File.ReadAllLines "./data.txt"

type State = Cell [] []

module State =
  let private join (s: string) (xs: string array) = String.Join(s, xs)

  let print state =
    state
    |> Array.map (
      Array.map (function
        | On -> "#"
        | Off -> " ")
      >> join ""
    )
    |> join "\n"
    |> printfn "%s"

  let initial (coords: Coord []) =
    let maxX = coords |> Array.maxBy fst |> fst
    let maxY = coords |> Array.maxBy snd |> snd

    Array.create (1 + maxY) (Array.create (1 + maxX) Off)
    |> Array.mapi (fun i row ->
      row
      |> Array.mapi (fun j _ ->
        if coords |> Array.contains (j, i) then
          On
        else
          Off))

  let private joinRows a b =
    a
    |> Array.mapi (fun i x ->
      match x, b |> Array.tryItem i with
      | On, Some On -> On
      | On, Some Off -> On
      | On, None -> On
      | Off, Some On -> On
      | Off, Some Off -> Off
      | Off, None -> Off)

  let doFold (state: State) fold =
    match fold with
    | Up x ->
      // take rows below x and reflect them on x
      state
      |> Array.mapi (fun i row ->
        if i < x then
          let d = x - i

          match state |> Array.tryItem (x + d) with
          | None -> row
          | Some r -> joinRows row r
        else
          row |> Array.map (fun _ -> Off))

    | Left x ->
      state
      |> Array.map (fun row -> joinRows row[0 .. x - 1] (row[x + 1 .. row.Length - 1] |> Array.rev))


let doFolds state folds = folds |> Array.fold State.doFold state

let partOne input =
  let coords, folds = Input.parse input
  let initialState = coords |> State.initial

  doFolds initialState [| folds[0] |]
  |> Array.sumBy (fun row ->
    row
    |> Array.sumBy (function
      | On -> 1
      | Off -> 0))

let partTwo input =
  let coords, folds = Input.parse input
  let initialState = coords |> State.initial
  doFolds initialState folds |> State.print

partOne Input.testInput
|> printfn "Part I (test): %i"

partOne Input.realInput
|> printfn "Part I (real): %i"

printfn "Part I: "
partTwo Input.realInput
