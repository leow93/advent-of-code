open System
open System.IO

let inputFile = File.ReadAllLines "./src/2017/02/input.txt"

let parseLine (line: string) =
  line.Split()
  |> Array.choose (fun x -> if x = "" then None else x |> int |> Some)

let input = inputFile |> Array.map parseLine

let solve (f: int array -> int) grid = grid |> Array.sumBy f

let partOne = input |> solve (fun row -> (row |> Array.max) - (row |> Array.min))

partOne |> printfn "Part I: %i"

let findDivision (row: int array) =
  let rec loop idx =
    match row |> Array.tryItem idx with
    | None -> failwith "Exhausted the list."
    | Some x ->
      let result = row |> Array.removeAt idx |> Array.choose(fun y ->
          if x % y = 0 then
            Some (x / y)
          elif y % x = 0 then
            Some (y / x)
          else
            None
        )
      
      match result with
      | [||] -> loop (idx + 1)
      | [|x|] -> x
      | _ -> failwith "Unexpected number of divisors"
  
  loop 0
      
let partTwo = input |> solve findDivision
partTwo |> printfn "Part II: %i"