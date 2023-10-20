open System

let testInput = "3,4,3,1,2"
let realInput = System.IO.File.ReadAllText "./data.txt"

let split (sep: string) (s: string) = s.Split sep

let empty =
  Map.ofList
    [ (0, 0L)
      (1, 0L)
      (2, 0L)
      (3, 0L)
      (4, 0L)
      (5, 0L)
      (6, 0L)
      (7, 0L)
      (8, 0L) ]

let nextVal map i =
  map |> Map.tryFind i |> Option.defaultValue 0L

let simulateDay map =
  map
  |> Map.add 0 (nextVal map 1)
  |> Map.add 1 (nextVal map 2)
  |> Map.add 2 (nextVal map 3)
  |> Map.add 3 (nextVal map 4)
  |> Map.add 4 (nextVal map 5)
  |> Map.add 5 (nextVal map 6)
  |> Map.add 6 ((nextVal map 7) + (nextVal map 0))
  |> Map.add 7 (nextVal map 8)
  |> Map.add 8 (nextVal map 0)

let rec simulateDays count map =
  match count with
  | 0 -> map
  | n -> simulateDays (n - 1) (simulateDay map)

let incrementKey map key =
  match key with
  | Some x ->
    map
    |> Map.change x (function
      | Some v -> Some(v + 1L)
      | None -> Some 1L)
  | None -> map

module Codec =
  let parseInt (x: string) =
    match Int32.TryParse x with
    | true, n -> Some n
    | _ -> None

  let decode (input: string) =
    input |> split "," |> Array.map parseInt |> Array.fold incrementKey empty

let howManyAfterNDays n input =
  input
  |> Codec.decode
  |> simulateDays n
  |> Map.fold (fun count _ v -> count + v) 0L

let after80Days = howManyAfterNDays 80

let after256Days = howManyAfterNDays 256

realInput |> after80Days |> printfn "Part I: %i"

realInput |> after256Days |> printfn "Part II: %i"
