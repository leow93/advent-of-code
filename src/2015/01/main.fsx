let readFile = System.IO.File.ReadAllText

let getNextFloor floor ch =
  match ch with
  | '(' -> floor + 1
  | ')' -> floor - 1
  | _ -> floor

let partOne file =
  readFile file
  |> (fun s -> s.ToCharArray())
  |> Array.fold getNextFloor 0

partOne "./data.txt" |> printfn "Part I: %i"

let partTwo file =
  let data =
    readFile file |> (fun s -> s.ToCharArray())

  let rec loop floor position =
    let nextFloor =
      match data |> Array.tryItem position with
      | None -> failwith "out of bounds"
      | Some ch -> getNextFloor floor ch

    if nextFloor = -1 then
      position + 1
    else
      loop nextFloor (position + 1)

  loop 0 0

partTwo "./data.txt" |> printfn "Part II: %i"
