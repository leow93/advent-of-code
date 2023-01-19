open System.Text.RegularExpressions

let doubleQuote, backslash, backslashQuote, doubleBackslash =
  "\"", "\\", "\\\"", "\\\\"

let unescape (s: string) =
  Regex.Replace(
    s
      .Substring(1, s.Length - 2)
      .Replace(backslash, doubleQuote)
      .Replace(doubleBackslash, "?"),
    @"\\x[0-9a-f]{2}",
    "?"
  )

let escape (s: string) =
  doubleQuote
  + s
    .Replace(backslash, doubleBackslash)
    .Replace(doubleQuote, backslashQuote)
  + doubleQuote

let input =
  "./data.txt" |> System.IO.File.ReadAllLines

let sumDiffLengths strings (f: string -> string * string) =
  strings
  |> Seq.map f
  |> Seq.sumBy (fun (a, b) -> a.Length - b.Length)

sumDiffLengths input (fun s -> (s, unescape s))
|> printfn "a: %d"

sumDiffLengths input (fun s -> (escape s, s))
|> printfn "b: %d"
