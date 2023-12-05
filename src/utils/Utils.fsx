module Input =
  let readText () = stdin.ReadToEnd()

  let readLines = readText >> (fun x -> x.Split("\n"))
