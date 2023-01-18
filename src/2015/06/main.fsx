open System.Text.RegularExpressions

let readLines = System.IO.File.ReadAllLines

type Light =
  | On
  | Off

type Grid = Light array array

type Instruction =
  | TurnOn of ((int * int) * (int * int))
  | TurnOff of ((int * int) * (int * int))
  | Toggle of ((int * int) * (int * int))

module Instruction =
  let private parseStringTuple (s: string) =
    match s.Split "," with
    | [| a; b |] -> (int a, int b)
    | _ -> failwithf "unexpected format %s" s

  let parse (s: string) =
    let regex =
      "(\d{1,3},\d{1,3}) through (\d{1,3},\d{1,3})"

    Regex.Matches(s, regex)
    |> Seq.map (fun x -> (x.Groups[1].Value |> parseStringTuple, x.Groups[2].Value |> parseStringTuple))
    |> Seq.map (fun x ->
      if s.StartsWith "turn on" then
        TurnOn x
      else if s.StartsWith "turn off" then
        TurnOff x
      else
        Toggle x)
    |> Seq.head

module StatePartOne =
  let initialState =
    Array.init 1000 (fun _ -> Array.init 1000 (fun _ -> Off))

  let private update state (x1, y1) (x2, y2) fn =
    state
    |> Array.mapi (fun i row ->
      row
      |> Array.mapi (fun j light ->
        if (x1 <= i && i <= x2) && (y1 <= j && j <= y2) then
          fn light
        else
          light))

  let evolve state instruction =
    match instruction with
    | TurnOn (a, b) -> update state a b (fun _ -> On)
    | TurnOff (a, b) -> update state a b (fun _ -> Off)
    | Toggle (a, b) ->
      update state a b (fun value ->
        match value with
        | On -> Off
        | Off -> On)

let data =
  readLines "./data.txt"
  |> Array.map Instruction.parse

let partOne () =
  data
  |> Array.fold StatePartOne.evolve StatePartOne.initialState
  |> Array.sumBy (fun row ->
    row
    |> Array.sumBy (fun light ->
      match light with
      | On -> 1
      | _ -> 0))

partOne () |> printfn "part one: %i"

module StatePartTwo =
  let initialState =
    Array.init 1000 (fun _ -> Array.init 1000 (fun _ -> 0))

  let private update state (x1, y1) (x2, y2) fn =
    state
    |> Array.mapi (fun i row ->
      row
      |> Array.mapi (fun j light ->
        if (x1 <= i && i <= x2) && (y1 <= j && j <= y2) then
          fn light
        else
          light))

  let evolve state instruction =
    match instruction with
    | TurnOn (a, b) -> update state a b (fun x -> x + 1)
    | TurnOff (a, b) -> update state a b (fun x -> if x > 0 then x - 1 else 0)
    | Toggle (a, b) -> update state a b (fun x -> x + 2)

let partTwo () =
  data
  |> Array.fold StatePartTwo.evolve StatePartTwo.initialState
  |> Array.sumBy (fun row -> row |> Array.sum)
  
partTwo () |> printfn "part two: %i"