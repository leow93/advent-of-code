let readLines = System.IO.File.ReadAllLines

module Rectangle =
  let parse (s: string) =
    match s.Split "x" with
    | [| l; w; h |] -> (int l, int w, int h) |> Some
    | _ -> None

  let private area (l, w, h) = (2 * l * w) + (2 * w * h) + (2 * h * l)

  let requiredWrappingPaper (l, w, h) =
    let slack =
      [ l; w; h ]
      |> List.sort
      |> (fun xs ->
        match xs with
        | [ a; b; _ ] -> a * b
        | _ -> 0)

    slack + area (l, w, h)

  let requiredRibbon (l, w, h) =
    let wrap =
      [ l; w; h ]
      |> List.sort
      |> (fun xs ->
        match xs with
        | [ a; b; _ ] -> (2 * a) + (2 * b)
        | _ -> 0)

    let bow = l * w * h
    wrap + bow

let partOne file =
  readLines file
  |> Array.choose Rectangle.parse
  |> Array.map Rectangle.requiredWrappingPaper
  |> Array.sum

partOne "./data.txt" |> printfn "Part I: %i"

let partTwo file =
  readLines file
  |> Array.choose Rectangle.parse
  |> Array.map Rectangle.requiredRibbon
  |> Array.sum

partTwo "./data.txt" |> printfn "Part II: %i"
