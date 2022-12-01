let readLines = System.IO.File.ReadAllLines
type State = int list
let evolve state line =
  match line with
  | "" -> state @ [ 0 ]
  | str ->
    match state with
    | [] -> [ int str ]
    | [ x ] -> [ x + int str ]
    | xs ->
      xs
      |> List.mapi (fun i x ->
        if i <> (xs |> List.length) - 1 then
          x
        else
          x + int str)

let runPart1 file =
  readLines file
  |> List.ofArray
  |> List.fold evolve List.empty
  |> List.sortDescending
  |> List.head
  
let runPart2 file =
  readLines file
  |> List.ofArray
  |> List.fold evolve List.empty
  |> List.sortDescending
  |> List.take 3
  |> List.sum

printfn "Test"
runPart1 "./test.txt" |> printf "Part I: %A\n"
runPart2 "./test.txt" |> printf "Part II: %A\n"

printfn "\nActual"
runPart1 "./data.txt" |> printf "Part I: %A\n"
runPart2 "./data.txt" |> printf "Part II: %A\n"