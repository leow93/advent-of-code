module Md5 =
  open System.Security.Cryptography
  let private getByteArray (s: string) = System.Text.Encoding.ASCII.GetBytes s

  let hash (s: string) =
    getByteArray s
    |> MD5.Create().ComputeHash
    |> Seq.map (fun c -> c.ToString "X2")
    |> Seq.reduce (+)


let findHashStartingWithX (x: string) key =
  let rec loop number =
    let hash = $"{key}{number}" |> Md5.hash

    if hash.StartsWith x then
      number
    else
      loop (number + 1)

  loop 1

let partOne = findHashStartingWithX "00000"
let partTwo = findHashStartingWithX "000000"

partOne "bgvyzdsv" |> printfn "Part I: %i"
partTwo "bgvyzdsv" |> printfn "Part II: %i"
