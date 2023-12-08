module Input =
  let readText () = stdin.ReadToEnd()

  let readLines = readText >> (fun x -> x.Split("\n"))


module Strings =
  let toCharArray (x: string) = x.ToCharArray() 
  let toStringArray = toCharArray >> Array.map string
  let split (sep: string) (s: string) = s.Split(sep)
