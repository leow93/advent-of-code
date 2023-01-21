open System.Text.Json
open System.Text.Json.Nodes

let readFile = System.IO.File.ReadAllText

let parseJson (file: string) = readFile file |> JsonDocument.Parse

let rec countNumbers (json: JsonElement) ignoreRed =
  match json.ValueKind with
  | JsonValueKind.Number -> json.GetInt32()
  | JsonValueKind.Array ->
    (json.EnumerateArray()
     |> Seq.fold (fun x y -> x + countNumbers y ignoreRed) 0)
  | JsonValueKind.Object ->
    let seq = json.EnumerateObject()

    match ignoreRed with
    | true when
      seq
      |> Seq.forall (fun x -> x.Value.ToString() <> "red")
      ->
      seq
      |> Seq.fold (fun x y -> x + countNumbers y.Value ignoreRed) 0
    | true -> 0
    | false ->
      seq
      |> Seq.fold (fun x y -> x + countNumbers y.Value ignoreRed) 0


  | _ -> 0

let doc = parseJson "./data.json"

let partOne () = countNumbers doc.RootElement false

let partTwo () = countNumbers doc.RootElement true

partOne () |> printfn "part I: %i"
partTwo () |> printfn "part II: %i"
