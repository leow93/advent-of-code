open System

let readLines file = System.IO.File.ReadAllLines file

type Bit =
  | One
  | Zero

let parseLine (line: string) =
  line.ToCharArray()
  |> Array.map (fun x -> x.ToString())
  |> Array.map (fun x ->
    match x with
    | "1" -> One
    | "0" -> Zero
    | _ -> failwithf "Unknown input %s" x)

let mostCommon xs =
  let rec loop (map: Map<Bit, int>) i =
    match xs |> Array.tryItem i with
    | None -> map
    | Some x ->
      let nextMap =
        map
        |> Map.change x (function
          | Some v -> Some(v + 1)
          | None -> Some 1)

      loop nextMap (i + 1)

  loop Map.empty 0 |> Map.toList |> List.maxBy snd |> fst

let leastCommon xs =
  let rec loop (map: Map<Bit, int>) i =
    match xs |> Array.tryItem i with
    | None -> map
    | Some x ->
      let nextMap =
        map
        |> Map.change x (function
          | Some v -> Some(v + 1)
          | None -> Some 1)

      loop nextMap (i + 1)

  loop Map.empty 0 |> Map.toList |> List.minBy snd |> fst

let foldBit s b =
  match b with
  | One -> s + "1"
  | Zero -> s + "0"

let toDecimal x = Convert.ToInt64(x, 2)

let bitsToDecimal (bits: Bit array) = bits |> Array.fold foldBit "" |> toDecimal

let gammaRate xs =
  xs |> Array.map mostCommon |> bitsToDecimal
let epsilonRate xs =
  xs
  |> Array.map leastCommon
  |> bitsToDecimal

let partOne file =
  let input = readLines file |> Array.map parseLine |> Array.transpose
  let gamma = input |> gammaRate 
  let epsilon = input |> epsilonRate 
  gamma * epsilon


let partTwo file =
  let input = file |> readLines |> Array.map parseLine

  let rec calculate filter (arr: Bit array array) i =
    match arr with
    | [| x |] -> x
    | _ ->
      let zeroes, ones =
        arr
        |> Array.fold
          (fun state item ->
            if item[i] = Zero then
              ((state |> fst) + 1, state |> snd)
            else
              (state |> fst, (state |> snd) + 1)

          )
          (0, 0)

      let filtered = arr |> Array.filter (filter zeroes ones i)

      calculate filter filtered (i + 1)


  let o2Filter zeroes ones i (digits: Bit array) =
    (zeroes > ones && digits[i] = Zero) || (zeroes <= ones && digits[i] = One)

  let co2Filter zeroes ones i (digits: Bit array) =
    (zeroes <= ones && digits[i] = Zero) || (ones < zeroes && digits[i] = One)

  (calculate o2Filter input 0 |> bitsToDecimal) * (calculate co2Filter input 0 |> bitsToDecimal)


"./test.txt" |> partOne |> printfn "Part I (test): %i"
"./data.txt" |> partOne |> printfn "Part I: %i"

"./test.txt" |> partTwo |> printfn "Part II (test): %i"
"./data.txt" |> partTwo |> printfn "Part II: %i"
