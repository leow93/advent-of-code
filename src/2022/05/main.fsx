open System.Collections.Generic

let read = System.IO.File.ReadAllText

let split (separator: string) (s: string) = separator |> s.Split

let parseFile file =
  let contents = read file |> split "\n\n"

  match contents with
  | [| grid; commands |] -> Some(grid, commands)
  | _ -> None

type Count = int
type FromIndex = int
type ToIndex = int
type Command = Move of (Count * FromIndex * ToIndex)

module Command =
  let parse (c: string) =
    match c with
    | "" -> None
    | s ->
      match split " " s with
      | [| _; count; _; fromColumn; _; toColumn |] -> Some(Move(int count, int fromColumn - 1, int toColumn - 1))
      | _ -> None

let resultString (dict: Dictionary<_, char []>) =
  let mutable result = ""

  for v in dict.Values do
    match v |> Array.tryItem 0 with
    | Some x -> result <- result + x.ToString()
    | None -> ()

  result

module State =
  let parse grid =
    let state =
      grid
      |> split "\n"
      |> Array.map (fun x -> x.ToCharArray())
      |> Array.transpose
      |> Array.map (Array.filter (fun x -> x <> ']' && x <> '[' && x <> ' '))
      |> Array.filter (fun c -> c.Length > 0)
      |> Array.map (fun c -> c[0 .. c.Length - 2])

    let dict = Dictionary<int, char []>()

    for i in 0 .. state.Length - 1 do
      dict[i] <- state[i]

    dict

let partOne file =
  match parseFile file with
  | None -> failwith "Failed to parse file"
  | Some (grid, commands) ->
    let state = State.parse grid

    let commands =
      split "\n" commands |> Array.choose Command.parse

    commands
    |> Array.iter (fun (Move (count, fromIdx, toIdx)) ->
      for _ in 1..count do
        match state[fromIdx] with
        | [||] -> ()
        | boxes ->
          match boxes |> Array.tryItem 0 with
          | None -> ()
          | Some box ->
            let newToColumn =
              state[toIdx] |> Array.append [| box |]

            let newFromColumn =
              state[fromIdx] |> Array.tail

            state[toIdx] <- newToColumn
            state[fromIdx] <- newFromColumn)

    resultString state

let partTwo file =
  match parseFile file with
  | None -> failwith "Failed to parse file"
  | Some (grid, commands) ->
    let state = State.parse grid

    let commands =
      split "\n" commands |> Array.choose Command.parse

    commands
    |> Array.iter (fun (Move (count, fromIdx, toIdx)) ->
      let boxes =
        state[fromIdx] |> Array.take count

      let newToColumn =
        state[toIdx] |> Array.append boxes

      let newFromColumn =
        state[fromIdx] |> Array.removeManyAt 0 count

      state[toIdx] <- newToColumn
      state[fromIdx] <- newFromColumn)

    resultString state



printfn "Test"
partOne "./test.txt" |> printf "Part I: %s\n"
partTwo "./test.txt" |> printf "Part II: %s\n"

printfn "\nActual"
partOne "./data.txt" |> printf "Part I: %s\n"
partTwo "./data.txt" |> printf "Part II: %s\n"
