module Input =
  let readText () = stdin.ReadToEnd()

  let readLines = readText >> (fun x -> x.Split("\n"))


module Strings =
  let toCharArray (x: string) = x.ToCharArray()
  let toStringArray = toCharArray >> Array.map string
  let split (sep: string) (s: string) = s.Split(sep)

  let replace (old: string) (x: string) (s: string) = s.Replace(old, x)

module Maths =
  let rec private greatestCommonDivisor x y =
    match x with
    | 0L -> y
    | _ when y = 0L -> x
    | _ -> greatestCommonDivisor y (x % y)

  let private lowestCommonMultiple x y = x * y / (greatestCommonDivisor x y)

  let rec lcm xs =
    match xs with
    | [ x; y ] -> lowestCommonMultiple x y
    | x :: xs -> lcm [ x; (lcm xs) ]
    | [] -> failwith "Impossible"
