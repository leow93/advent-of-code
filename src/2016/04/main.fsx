open System

type Room =
  { SectorId: int
    EncryptedName: string
    Checksum: string }

module Input =
  let parseLine (line: string) =
    let [| rest; checksum |] =
      line.Split('[', ']')
      |> Array.filter (not << String.IsNullOrWhiteSpace)

    let lastDash = rest.LastIndexOf('-')

    let sectorId =
      rest.Substring(lastDash + 1) |> int

    let encryptedName =
      rest.Substring(0, lastDash)

    { EncryptedName = encryptedName
      SectorId = sectorId
      Checksum = checksum }

let toString (seq: 'a seq) = String.Join("", seq)

let fiveMostCommonLetters (str: string) =
  str.ToCharArray()
  |> Array.filter (fun ch -> ch <> '-')
  |> Array.fold
       (fun map char ->
         map
         |> Map.change char (fun count ->
           match count with
           | None -> Some 1
           | Some x -> Some(x + 1)))
       Map.empty
  |> Map.toList
  |> List.sortByDescending snd
  |> List.take 5
  |> List.map fst
  |> toString

let isRealRoom room =
  let letters =
    fiveMostCommonLetters room.EncryptedName

  letters = room.Checksum


let input =
  System.IO.File.ReadAllLines "./input.txt"

let rooms =
  input |> Array.map Input.parseLine

let partOne =
  rooms
  |> Array.filter isRealRoom
  |> Array.sumBy (fun x -> x.SectorId)

printfn "Part I: %i" partOne

let alphabet = { 'a' .. 'z' }

let nextChar ch =
  match ch with
  | '-'
  | ' ' -> ' '
  | 'z' -> 'a'
  | x ->
    let idx =
      match alphabet |> Seq.tryFindIndex (fun a -> a = x) with
      | Some idx -> idx + 1
      | None -> 0

    alphabet |> Seq.item idx

let decrypt (s: string) count =
  { 1..count }
  |> Seq.fold (fun chars _ -> chars |> Array.map nextChar) (s.ToCharArray())
  |> toString

let partTwo =
  rooms
  |> Array.filter isRealRoom
  |> Array.map (fun x ->
    let name =
      decrypt x.EncryptedName x.SectorId

    name, x.SectorId)
  |> Array.tryFind (fun (name, _) -> name.Contains "north")

printfn "Part II: %A" partTwo
