type Item =
  | Generator of string
  | Chip of string

  member x.Element =
    match x with
    | Generator s -> s
    | Chip s -> s

type Floor = Set<Item>

type State =
  { Elevator: int
    Floors: Floor [] }

  override x.ToString() =
    let byElement =
      x.Floors
      |> Seq.mapi (fun n items -> items |> Seq.map (fun i -> n, i))
      |> Seq.collect id
      |> Seq.groupBy (fun (_n, i) -> i.Element)
      |> Seq.map (fun (_e, itemOnFloors) ->
        match itemOnFloors |> Seq.toArray with
        | [| (i, Generator _); (j, _) |] -> i, j
        | [| (i, Chip _); (j, _) |] -> j, i
        | _ -> failwithf "oops")
      |> Seq.sort

    System.String.Join("", byElement)
    |> sprintf "%d-%s" x.Elevator


let input =
  System.IO.File.ReadAllLines "./input.txt"


let (|Empty|NotEmpty|) (floor: Floor) =
  if floor.Count = 0 then
    Empty
  else
    NotEmpty


let (|Fried|_|) (floor: Floor) =
  let chips, generators =
    floor
    |> Set.partition (function
      | Chip _ -> true
      | _ -> false)
    |> fun (chips, gens) -> chips |> Set.map (fun x -> x.Element), gens |> Set.map (fun x -> x.Element)

  let unmatchedChips =
    Set.difference chips generators

  if unmatchedChips.Count > 0 && generators.Count > 0 then
    Some unmatchedChips
  else
    None

let (|Success|Failed|InProgress|) { Floors = floors } =
  match floors with
  | [| Empty; Empty; Empty; NotEmpty |] -> Success
  | [| Fried _; _; _; _ |]
  | [| _; Fried _; _; _ |]
  | [| _; _; Fried _; _ |]
  | [| _; _; _; Fried _ |] -> Failed
  | _ -> InProgress
