let input = System.IO.File.ReadAllLines "./src/2017/04/input.txt"
let split (sep: string) (s: string) = s.Split sep

let passphrases =
  input |> Array.map (fun line -> line.Split() |> List.ofArray) |> List.ofArray

let noDuplicates (passphrase: string array) =
  passphrase |> Set.ofArray |> Set.count = passphrase.Length

let isUnique xs =
  let rec loop seen xs =
    match xs with
    | [] -> true
    | x :: xs ->
      if Set.contains x seen then
        false
      else
        loop (Set.add x seen) xs

  loop Set.empty xs

let solve mapper =
  Seq.map mapper >> Seq.filter isUnique >> Seq.length

passphrases |> solve id |> printfn "Part I: %i"

let sortString (s: string) = s |> Seq.sort |> System.String.Concat

passphrases |> solve (List.map sortString) |> printfn "Part II: %i"
// 131 too low
// 381 too high
