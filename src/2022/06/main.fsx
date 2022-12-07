let read = System.IO.File.ReadAllText

let getPositionOfUniqueBuffer size file =
  ((read file).ToCharArray()
   |> Seq.ofArray
   |> Seq.windowed size
   |> Seq.findIndex (fun chars ->
     match chars with
     | c ->
       c.Length = size
       && c |> Set.ofArray |> Set.count = size))
  + size

let partOne file = getPositionOfUniqueBuffer 4 file
let partTwo file = getPositionOfUniqueBuffer 14 file

printfn "Test"
partOne "./test.txt" |> printf "Part I: %A\n"
partTwo "./test.txt" |> printf "Part II: %A\n"

printfn "\nActual"
partOne "./data.txt" |> printf "Part I: %A\n"
partTwo "./data.txt" |> printf "Part II: %A\n"
