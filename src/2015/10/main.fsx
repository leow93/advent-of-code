let input = "1321131112" |> Seq.map(fun f -> int f - int '0') |> Seq.toArray

let countAdjacent =
  Seq.fold(fun s x ->
      match s with
      | [|n; c|]::tail when c = x -> [|n+1;c|]::tail
      | l -> [|1;x|]::l) []
    >> List.rev

let lookAndSay = countAdjacent >> Seq.collect id >> Seq.toArray

let getLengthAfterRepetitions repetitions = 
    [1..repetitions]
    |> Seq.fold (fun acc _ -> lookAndSay acc) input 
    |> Seq.length

getLengthAfterRepetitions 40 |> printfn "a: %d"
getLengthAfterRepetitions 50 |> printfn "b: %d"