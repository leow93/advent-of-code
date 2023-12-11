open System.Diagnostics
open System.IO
open Microsoft.FSharp.Control

let makeList init n = init n id

let arr = makeList Array.init

let list = makeList List.init

let a = arr 5000000
let l = list 5000000

let time name fn =
  let sw = Stopwatch()
  sw.Start()
  let result = fn ()
  sw.Stop()
  printfn name sw.ElapsedMilliseconds
  result

(fun () -> a[a.Length - 1]) |> time "arr %i ms"

(fun () -> l[l.Length - 1]) |> time "list %i ms"


type Eventually<'a> =
  | Done of 'a
  | NotYetDone of (unit -> Eventually<'a>)

module Eventually =
  let rec bind func expr =
    match expr with
    | Done v -> func v
    | NotYetDone work -> NotYetDone(fun () -> bind func (work ()))

  let result value = Done value

  let rec catch expr =
    match expr with
    | Done x -> result (Ok x)
    | NotYetDone work ->
      NotYetDone (fun () ->
        let res =
          try
            Ok(work ())
          with
          | exn -> Error exn

        match res with
        | Ok cont -> catch cont
        | Error exn -> result (Error exn))

  let delay func = NotYetDone(fun () -> func ())

  let step expr =
    match expr with
    | Done _ -> expr
    | NotYetDone func -> func ()

  let tryFinally expr compensation =
    catch expr
    |> bind (fun res ->
      compensation ()

      match res with
      | Ok value -> result value
      | Error exn -> raise exn)

  let tryWith exn handler =
    catch exn
    |> bind (function
      | Ok value -> result value
      | Error exn -> handler exn)

  let rec whileLoop pred body =
    if pred () then
      body |> bind (fun _ -> whileLoop pred body)
    else
      result ()

  let combine expr1 expr2 = expr1 |> bind (fun () -> expr2)

  let using (resource: #System.IDisposable) func =
    tryFinally (func resource) (fun () -> resource.Dispose())

  let forLoop (collection: seq<_>) func =
    let ie = collection.GetEnumerator()
    tryFinally (whileLoop (fun () -> ie.MoveNext()) (delay (fun () -> let value = ie.Current in func value))) (fun () -> ie.Dispose())

type EventuallyBuilder() =
  member x.Bind(comp, func) = Eventually.bind func comp
  member x.Return(value) = Eventually.result value
  member x.ReturnFrom(value) = value
  member x.Combine(expr1, expr2) = Eventually.combine expr1 expr2
  member x.Delay(func) = Eventually.delay func
  member x.Zero() = Eventually.result ()
  member x.TryWith(expr, handler) = Eventually.tryWith expr handler
  member x.TryFinally(expr, compensation) = Eventually.tryFinally expr compensation
  member x.For(coll: seq<_>, func) = Eventually.forLoop coll func
  member x.Using(resource, expr) = Eventually.using resource expr

let eventually = EventuallyBuilder()

let comp =
  eventually {
    for x in 1..2 do
      printfn $" x = %d{x}"
    return 3 + 4
  }
