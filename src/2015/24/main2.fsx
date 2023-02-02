open System
 
let rec combinations size list =
  let rec inner acc size set =
    seq {
      match size, set with
      | n, x :: xs ->
        if n > 0 then
          yield! inner (x :: acc) (n - 1) xs

        if n >= 0 then yield! inner acc n xs
      | 0, [] -> yield acc
      | _, [] -> ()
    }

  inner [] size list
  
let sum (list: int64 list) = list |> List.reduce (+)

let weights =
  System.IO.File.ReadAllLines "./data.txt"
  |> Seq.map int64
  |> Seq.toList

let groupsOfWeight packages weightPerGroup =
  seq {
    for groupSize in { 1 .. packages |> List.length } do
      for group in (combinations groupSize packages) do
        if group |> sum = weightPerGroup then
          yield group
  }

let sub xs ys =
  xs
  |> List.filter (fun x -> not (ys |> List.contains x))

let rec canGroup packages (numOfGroups: int64) weightPerGroup =
  if numOfGroups = 0L then
    packages |> List.length = 0
  else
    let mutable result = false

    for group in (groupsOfWeight packages weightPerGroup) do
      if canGroup (sub packages group) (numOfGroups - 1L) weightPerGroup then
        result <- true

    result

let qe (xs: int64 list) = xs |> List.reduce (*)

let idealFirstGroupQE packages (numOfGroups: int64) =
  let weightPerGroup =
    (packages |> sum) / numOfGroups

  let groups =
    groupsOfWeight packages weightPerGroup

  printfn "groups %A" (groups |> Seq.length)

  let rec inner idx (minQE: int64) (prevGroupSize: int64) =
    match groups |> Seq.tryItem idx with
    | None -> minQE
    | Some group ->
      if (minQE <> Int64.MaxValue
          && prevGroupSize < group.Length) then
        minQE
      else
        let candidateQE = qe group

        if (candidateQE < minQE
            && canGroup (sub packages group) (numOfGroups - 1L) weightPerGroup) then
          inner (idx + 1) candidateQE group.Length
        else
          inner (idx + 1) minQE group.Length

  inner 0 Int64.MaxValue Int64.MaxValue

let partOne = idealFirstGroupQE weights 3L

partOne |> printfn "Part I: %i"
