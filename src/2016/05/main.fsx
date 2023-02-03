open System
open System.Collections.Generic

module Md5 =
  open System.Security.Cryptography
  let private getByteArray (s: string) = System.Text.Encoding.ASCII.GetBytes s

  let hash (s: string) =
    getByteArray s
    |> MD5.Create().ComputeHash
    |> Seq.map (fun c -> c.ToString "X2")
    |> Seq.reduce (+)

let rec nextHash (startsWith: string) (key: string) number =
  let hash = $"{key}{number}" |> Md5.hash

  if hash.StartsWith startsWith then
    hash, number
  else
    nextHash startsWith key (number + 1)

let findHashesStartingWithX (x: string) (key: string) count =
  let rec loop list number =
    if list |> List.length = count then
      list
    else
      let hash = $"{key}{number}" |> Md5.hash

      if hash.StartsWith x then
        loop (list @ [ hash ]) (number + 1)
      else
        loop list (number + 1)


  loop [] 1


let toString (seq: 'a seq) = String.Join("", seq)

let doorId = "ffykfhsq"

let partOne =
  findHashesStartingWithX "00000" doorId 8
  |> List.map (fun x -> x[5])
  |> toString

partOne |> printfn "Part I: %A"

let partTwo =
  let mutable n = 0
  let password = Dictionary()

  while password.Count < 8 do
    let hash, number =
      nextHash "00000" doorId n

    match hash[5] |> string |> Int32.TryParse with
    | true, x when x < 8 ->
      if not (password.ContainsKey x) then
        password.Add(x, hash[6])
    | _ -> ()

    n <- number + 1

  password.Keys
  |> Seq.sort
  |> Seq.map (fun key -> password[key])
  |> toString


partTwo |> printfn "Part II: %A"
