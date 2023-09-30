open System

let input =
  System.IO.File.ReadAllText "./data.txt"

let tryInt (x: string) =
  match Int32.TryParse x with
  | true, x -> Some x
  | _ -> None

let parseInput (input: string) = input.Split "," |> Array.choose tryInt

let sum a b = a + b


let range a b =
  let d = b - a

  if d < 0 then
    [||]
  else
    Array.init (d + 1) id

let getFuelCosts fuelCostFn positions =
  range (Array.min positions) (Array.max positions)
  |> Array.fold
       (fun map position ->
         let ds =
           ResizeArray(List.init positions.Length (fun _ -> 0))

         for i in { 0 .. positions.Length - 1 } do
           ds[i] <- (fuelCostFn positions[i] position)

         map |> Map.add position (ds |> Seq.fold sum 0))
       Map.empty
  |> Map.toArray
  |> Array.sortBy snd
  |> Array.head

let diff a b = a - b |> abs

let sumFromOneToN n = n * (n + 1) / 2

let partOne =
  input |> parseInput |> getFuelCosts diff |> snd

partOne |> printfn "Part I: %i"

let partTwo =
  input |> parseInput |> getFuelCosts (fun a b -> diff a b |> sumFromOneToN) |> snd

partTwo |> printfn "Part II: %i"