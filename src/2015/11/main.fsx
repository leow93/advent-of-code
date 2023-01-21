module Password =
  let private alphabet = { 'a' .. 'z' }

  let private increment i =
    match i with
    | 25 -> 0
    | x -> x + 1

  let private asInt ch =
    alphabet |> Seq.findIndex (fun x -> x = ch)

  let private asChar i = alphabet |> Seq.item i

  let private nextChar (ch: char) = ch |> asInt |> increment |> asChar

  let rec nextPassword (s: string) =
    let chars = s.ToCharArray()

    match chars |> Array.tryLast with
    | None -> ""
    | Some last ->
      let nextCh = nextChar last

      if nextCh = 'a' then
        nextPassword (s[0 .. s.Length - 2]) + "a"
      else
        s[0 .. s.Length - 2] + $"{nextCh}"

  let private containsRun (password: string) =
    let ints =
      password.ToCharArray() |> Array.map asInt

    ints
    |> Array.windowed 3
    |> Array.exists (fun xs ->
      xs.Length = 3
      && (xs[2] = xs[1] + 1 && xs[1] = xs[0] + 1))

  let private excludes (chars: Set<char>) (password: string) =
    chars
    |> Set.fold (fun x char -> x && not (password.Contains char)) true

  let private containsTwoPairs (password: string) =
    let chars = password.ToCharArray()
    
    let indexedPairs = 
      chars
      |> Array.pairwise
      |> Array.indexed
      |> Array.filter (fun (_, (a, b)) -> a = b)
    
    indexedPairs |> Array.fold(fun map (i, pair) ->
       match map |> Map.tryFind (i - 1) with
       | None -> map |> Map.add i pair
       | Some _ -> map
      ) Map.empty
    |> Map.count >=2  

  let validate password =
    password |> containsRun
    && password |> excludes (Set.ofList [ 'i'; 'o'; 'l' ])
    && password |> containsTwoPairs

let getNextPassword password =
  let mutable valid = false
  let mutable password = password
  while (not valid) do
    password <- Password.nextPassword password
    valid <- Password.validate password
  
  password

let partOneAnswer = getNextPassword "hepxcrrq"

partOneAnswer |> printfn "Part I: %s"

partOneAnswer |> getNextPassword |> printfn "Part II: %s"
