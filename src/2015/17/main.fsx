let readLines = System.IO.File.ReadAllLines

let parseFile file = file |> readLines |> Array.map int

let allCombinations list =
  let rec comb accLst elemLst =
    match elemLst with
    | h :: t ->
      let next =
        [ h ] :: List.map (fun el -> h :: el) accLst
        @ accLst

      comb next t
    | _ -> accLst

  comb [] list

let partOne file total =
  let containers =
    parseFile file |> List.ofArray

  let permutations =
    containers |> allCombinations

  permutations
  |> List.filter (fun list -> list |> List.sum = total)
  |> List.length

let partTwo file total =
  let containers =
    parseFile file |> List.ofArray

  let permutations =
    containers |> allCombinations

  let waysToHoldTotal =
    permutations
    |> List.filter (fun list -> list |> List.sum = total)

  let minimumContainers =
    waysToHoldTotal
    |> List.map List.length
    |> List.min

  waysToHoldTotal
  |> List.filter (fun list -> list.Length = minimumContainers)
  |> List.length


partOne "./test.txt" 25
|> printfn "Part I (test): %A"

partOne "./data.txt" 150 |> printfn "Part I: %A"

partTwo "./test.txt" 25
|> printfn "Part II (test): %A"

partTwo "./data.txt" 150
|> printfn "Part II: %A"
