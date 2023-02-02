let parseFile file =
  System.IO.File.ReadAllLines file
  |> Seq.map int
  |> Seq.toList

let weights =
  System.IO.File.ReadAllLines "./data.txt"
  |> Seq.map int
  |> Seq.toList

let totalWeight = weights |> Seq.sum
let avgWeight = totalWeight / 3
printfn "%i" avgWeight

let inline quantumEntanglement x = x |> Seq.reduce (*)
  

let groups =
  let rec loop (total, acc) rest =
    seq {
      if total = avgWeight && List.length acc = 6 then
        yield acc
      else
        match rest with
        | [] -> ()
        | head :: tail ->
          yield! loop (head + total, head :: acc) tail
          yield! loop (total, acc) rest
    }

  loop (0, []) weights

groups
|> Seq.map quantumEntanglement
|> Seq.min
|> printfn "Part I: %i"
