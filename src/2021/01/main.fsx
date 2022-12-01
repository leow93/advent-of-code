let readLines = System.IO.File.ReadLines

type State = { Count: int; Value: int option }

let part1 =
  Seq.fold
    (fun state value ->
      match state.Value with
      | None -> { Count = 0; Value = Some value }
      | Some x when value > x ->
        { Count = state.Count + 1
          Value = Some value }
      | _ ->
        { Count = state.Count
          Value = Some value })
    { Count = 0; Value = None }


let part2 (data: int seq) =
  data
  |> Seq.windowed 3
  |> Seq.map (Seq.sum)
  |> part1
  |> printfn "%A"

let runPart1 file =
  readLines file
  |> Seq.map int
  |> part1
  |> printf "%A\n"

let runPart2 file =
  readLines file
  |> Seq.map int
  |> part2
  |> printf "%A\n"

printfn "Test"
runPart1 "./test.txt"
runPart2 "./test.txt"

printfn "Actual"
runPart1 "./data.txt"
runPart2 "./data.txt"
