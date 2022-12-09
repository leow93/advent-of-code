open System

let readLines = System.IO.File.ReadAllLines

let split (separator: string) (s: string) = s.Split(separator) |> List.ofArray

type Item =
  | File of File
  | Directory of Directory

and File = { name: string; size: int }

and Directory =
  { path: string
    children: ResizeArray<Item>
    parent: Directory option }

type Command =
  | Cd of string
  | Ls
  | Dir of string
  | File of File

module Command =
  let parse s =
    match split "$ " s with
    | [ ""; "ls" ] -> Ls
    | [ ""; maybeCd ] when maybeCd.StartsWith("cd ") ->
      match maybeCd.Substring 3 with
      | x -> Cd x
    | [ ""; _ ] -> failwithf "unknown: %s" s
    | [ x ] when x.StartsWith "dir " -> Dir(x.Substring 4)
    | [ x ] ->
      match split " " x with
      | [ size; name ] -> File { name = name; size = int size }
      | _ -> failwithf "unknown: %s" s
    | _ -> failwithf "unknown: %s" s

module DirectoryState =
  let initial () =
    { path = ""
      children = ResizeArray()
      parent = None }

  let evolve state command =
    match command with
    | Ls
    | Dir _ -> state
    | File file ->
      let containsFile =
        state.children |> Seq.contains (Item.File file)

      if not containsFile then
        state.children.Add(Item.File file)

      state
    | Cd dir ->
      match dir with
      | ".." -> state.parent |> Option.get
      | path ->
        let dir =
          state.children
          |> Seq.tryPick (function
            | Directory dir when dir.path = path -> Some dir
            | _ -> None)

        match dir with
        | Some dir -> dir
        | None ->
          let dir =
            { path = path
              parent = Some state
              children = ResizeArray() }

          state.children.Add(Item.Directory dir)
          dir

let rec findDirectoryRoot tree =
  match tree.parent with
  | None -> tree
  | Some x -> findDirectoryRoot x

let directories =
  (function
  | Directory dir -> Some dir
  | _ -> None)

let rec calculateSize dir =
  match dir with
  | Item.Directory dir -> Seq.sumBy calculateSize dir.children
  | Item.File f -> f.size

let partOne file =
  let rec handle (tree: Directory) =
    let size =
      tree.children |> Seq.sumBy calculateSize

    let sizeOfDirectories =
      tree.children
      |> Seq.choose directories
      |> Seq.sumBy handle

    if size > 100000 then
      sizeOfDirectories
    else
      size + sizeOfDirectories

  readLines file
  |> Array.map Command.parse
  |> Array.fold DirectoryState.evolve (DirectoryState.initial ())
  |> findDirectoryRoot
  |> handle

let partTwo file =
  let requiredSpace = 30000000
  let totalSpace = 70000000

  let rec dirsWithSufficientSpace space (tree: Directory) =
    let size =
      tree.children |> Seq.sumBy calculateSize

    let sizeOfDirectories =
      tree.children
      |> Seq.choose directories
      |> Seq.collect (dirsWithSufficientSpace space)
      |> Seq.toArray

    if size >= space then
      Array.concat [| [| size |]
                      sizeOfDirectories |]
    else
      sizeOfDirectories

  let rec handle directory =
    let used =
      calculateSize (Item.Directory directory)

    let unused = totalSpace - used
    let spaceToDelete = requiredSpace - unused

    dirsWithSufficientSpace spaceToDelete directory
    |> Array.sort
    |> Array.head

  readLines file
  |> Array.map Command.parse
  |> Array.fold DirectoryState.evolve (DirectoryState.initial ())
  |> findDirectoryRoot
  |> handle

printfn "Test"
partOne "./test.txt" |> printf "Part I: %i\n"
partTwo "./test.txt" |> printf "Part II: %i\n"

printfn "\nActual"
partOne "./data.txt" |> printf "Part I: %i\n"
partTwo "./data.txt" |> printf "Part II: %i\n"
