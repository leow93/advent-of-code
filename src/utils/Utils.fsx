module Input =
  let private readAll () = stdin.ReadToEnd()

  let private readLines () = readAll().Split("\n")

  let text = readAll ()

  let lines = readLines ()
