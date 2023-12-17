#load "../../utils/Utils.fsx"

open Utils
let input = Input.readText ()

let add a b = a + b
let multiply a b = a * b
let remainder a b = b % a

let hash (s: string) =
  s.ToCharArray()
  |> Array.fold (fun curr ch -> curr |> add (int ch) |> multiply 17 |> remainder 256) 0

let partOne input =
  input |> Strings.replace "\n" "" |> Strings.split "," |> Array.sumBy hash

input |> partOne |> printfn "Part 1: %d"

type Lens = { label: string; power: int }
type Box = { id: int; lenses: Lens list }
type Step = { boxId: int; operation: Operation }

and Operation =
  | RemoveLens of string
  | ReplaceLens of Lens


let parseInput input =
  input
  |> Strings.replace "\n" ""
  |> Strings.split ","
  |> Array.choose (fun s ->
    if s.Contains '-' then
      match s.Split '-' with
      | [| label;_ |] ->
        { boxId = hash label
          operation = RemoveLens label }
        |> Some
      | _ -> None
    else
      match s.Split '=' with
      | [| label; value |] ->
        { boxId = hash label
          operation = ReplaceLens { label = label; power = int value } }
        |> Some
      | _ -> None)

let partTwo input =
  let boxes = Array.init 256 (fun i -> { id = i; lenses = [] })
  let steps = input |> parseInput

  let rec loop i =
    match steps |> Array.tryItem i with
    | None -> boxes
    | Some step ->
      let box = boxes[step.boxId]
      match step.operation with
      | RemoveLens label ->
        let lenses = box.lenses |> List.filter (fun l -> l.label <> label)
        boxes.[step.boxId] <- { box with lenses = lenses }
        loop (i + 1)
      | ReplaceLens x ->
        match box.lenses |> List.tryFindIndex (fun l -> l.label = x.label) with
        | Some idx ->
          let lenses = box.lenses |> List.mapi (fun i l -> if i = idx then x else l)
          boxes[step.boxId] <- { box with lenses = lenses }
          loop (i + 1)
        | None ->
          boxes[step.boxId] <- { box with lenses = List.append box.lenses [x] }
          loop (i + 1)

  let boxes = loop 0
  boxes
  |> Array.sumBy (fun box ->
    box.lenses
    |> List.indexed
    |> List.sumBy (fun (i, l) -> (1 + box.id) * (i + 1) * l.power))

input |> partTwo |> printfn "Part 2: %A"
