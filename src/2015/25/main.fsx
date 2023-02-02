open System

let triangleNumberAt row col =
  let side = row + col - 1
  ((side * (side + 1)) / 2) - row


let rec modExp (b: float) (exp: float) (modulus: float) =
  if exp = 0. then
    1.
  elif exp % 2. = 0.  then
    Math.Pow(modExp b (exp / 2.) modulus, 2.) % modulus
  else
    (b * (modExp b (exp - 1.) modulus)) % modulus

let partOne =
  let row = 2978
  let col = 3083
  let firstCode = float 20151125
  let base' = 252533
  let modulus = float  33554393
  let exp = triangleNumberAt row col
  
  ((modExp base' exp modulus) * firstCode) % modulus 

partOne |> printfn "part I: %f"