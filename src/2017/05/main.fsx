let testInput =
  """
0
3
0
1
-3
"""

let input = System.IO.File.ReadAllText "./src/2017/05/input.txt"

let parseInput (input: string) =
  input.Split("\n")
  |> Array.choose (fun x -> if x = "" then None else Some(int x))
  |> List.ofArray

let moveThroughInstructions nextOffset instructions =
  let rec loop instructions position steps =
    match instructions |> List.tryItem position with
    | None -> steps
    | Some x ->
      let newInstructions = instructions |> List.updateAt position (nextOffset x)
      loop newInstructions (position + x) (steps + 1)

  loop instructions 0 0

let addOne x = x + 1

let partOne input =
  input |> parseInput |> (moveThroughInstructions addOne)

printfn "Part I (test): %i" (partOne testInput)
printfn "Part I %i" (partOne input)

let nextOffset x =
  if x >= 3 then x - 1 else x + 1
let partTwo input =
  input |> parseInput |> (moveThroughInstructions nextOffset)

printfn "Part II (test): %i" (partTwo testInput)
printfn "Part II %i" (partTwo input)
