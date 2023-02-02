open System

let input =
  IO.File.ReadAllLines "./data.txt"

let splitBy (c: string) f (str: string) =
  str.Split([| c |], StringSplitOptions.None) |> f

let parseReplacements lines =
  let lineArr = lines |> Seq.toArray

  let replacements =
    lineArr
    |> Array.take (lineArr.Length - 2)
    |> Array.map (splitBy " => " (fun t -> t.[0], t.[1]))
    |> Array.groupBy fst
    |> Map.ofArray
    |> Map.map (fun _ v -> v |> Array.map snd)

  let molecule = Array.last lineArr

  replacements, molecule

let replacements, molecule =
  parseReplacements input
let partOne =
  [ 0 .. molecule.Length - 1 ]
  |> Seq.collect (fun i ->
    if i = molecule.Length - 1 then
      [ i, 1 ]
    else
      [ i, 1; i, 2 ])
  |> Seq.collect (fun (i, l) ->
    let k = molecule.Substring(i, l)

    if Map.containsKey k replacements then
      replacements.[k]
      |> Array.map (fun r ->
        molecule.Substring(0, i)
        + r
        + molecule.Substring(min (molecule.Length - 1) (l + i)))
    else
      [||])
  |> Seq.distinct
  |> Seq.length

printfn "Part I: %i" partOne

let partTwo =
  let totalElements = molecule |> Seq.filter (fun c -> System.Char.IsUpper c) |> Seq.length
  let totalRn = molecule |> Seq.pairwise |> Seq.filter (fun (a, b) -> a = 'R' && b = 'n') |> Seq.length
  let totalAr = molecule |> Seq.pairwise |> Seq.filter (fun (a, b) -> a = 'A' && b = 'r') |> Seq.length
  let totalY = molecule |> Seq.filter (fun a -> a = 'Y') |> Seq.length
  totalElements - totalRn - totalAr - 2 * totalY - 1

printfn "Part II: %i" partTwo