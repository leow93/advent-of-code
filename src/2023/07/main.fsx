#load "../../utils/Utils.fsx"

open Utils

type Bid = int
type CardNumber = int

type Card =
  | Ace
  | King
  | Queen
  | Jack
  | Joker
  | Number of CardNumber

type Hand = Card list

type HandRank =
  | FiveOfAKind of Hand
  | FourOfAKind of Hand
  | FullHouse of Hand
  | ThreeOfAKind of Hand
  | TwoPair of Hand
  | Pair of Hand
  | HighCard of Hand

module Ranking =
  let private cardScore =
    function
    | Ace -> 14
    | King -> 13
    | Queen -> 12
    | Jack -> 11
    | Number x -> x
    | Joker -> 1

  let private cmp (a: Hand) (b: Hand) =
    let rec inner i =
      match a |> List.tryItem i, b |> List.tryItem i with
      | Some a, Some b ->
        let x = compare (cardScore a) (cardScore b)
        if x = 0 then inner (i + 1) else x
      | _ -> 0

    inner 0

  let sort a b =
    let handA = a |> fst
    let handB = b |> fst
    
    match handA, handB with
    | FiveOfAKind a, FiveOfAKind b -> cmp a b
    | FiveOfAKind _, _ -> 1
    | _, FiveOfAKind _ -> -1
    | FourOfAKind a, FourOfAKind b -> cmp a b
    | FourOfAKind _, _ -> 1
    | _, FourOfAKind _ -> -1
    | FullHouse a, FullHouse b -> cmp a b
    | FullHouse _, _ -> 1
    | _, FullHouse _ -> -1
    | ThreeOfAKind a, ThreeOfAKind b -> cmp a b
    | ThreeOfAKind _, _ -> 1
    | _, ThreeOfAKind _ -> -1
    | TwoPair a, TwoPair b -> cmp a b
    | TwoPair _, _ -> 1
    | _, TwoPair _ -> -1
    | Pair a, Pair b -> cmp a b
    | Pair _, _ -> 1
    | _, Pair _ -> -1
    | HighCard a, HighCard b -> cmp a b

module Parsing =
  let private countJokersInHand (x: HandRank) =
    let eqJoker x = x = Joker

    match x with
    | FiveOfAKind xs -> xs |> List.filter eqJoker |> List.length
    | FourOfAKind xs -> xs |> List.filter eqJoker |> List.length
    | FullHouse xs -> xs |> List.filter eqJoker |> List.length
    | ThreeOfAKind xs -> xs |> List.filter eqJoker |> List.length
    | TwoPair xs -> xs |> List.filter eqJoker |> List.length
    | Pair xs -> xs |> List.filter eqJoker |> List.length
    | HighCard xs -> xs |> List.filter eqJoker |> List.length

  let private optimiseJokers hand =
    let jokerCount = countJokersInHand hand

    if jokerCount = 0 then
      hand
    else
      match hand with
      | FiveOfAKind _ -> hand // can't beat the best
      | FourOfAKind xs ->
        match jokerCount with
        | 4
        | 1 -> FiveOfAKind xs
        | _ -> hand
      | FullHouse xs ->
        match jokerCount with
        | _ -> FiveOfAKind xs
      | ThreeOfAKind xs ->
        match jokerCount with
        | 3 -> FourOfAKind xs
        | 2 -> hand
        | 1 -> FourOfAKind xs
        | _ -> hand
      | TwoPair xs ->
        match jokerCount with
        | 1 -> FullHouse xs
        | 2 -> FourOfAKind xs
        | _ -> hand
      | Pair xs ->
        match jokerCount with
        | 1 -> ThreeOfAKind xs
        | 2 -> ThreeOfAKind xs
        | _ -> hand
      | HighCard xs -> Pair xs

  let private rank hand =
    let set = hand |> Set.ofList

    let baseRank =
      match set.Count with
      | 1 -> FiveOfAKind hand
      | 2 ->
        if hand |> List.countBy id |> List.exists (fun (_, count) -> count = 4) then
          FourOfAKind hand
        else
          FullHouse hand
      | 3 ->
        if hand |> List.countBy id |> List.exists (fun (_, count) -> count = 3) then
          ThreeOfAKind hand
        else
          TwoPair hand
      | 4 -> Pair hand
      | _ -> HighCard hand

    optimiseJokers baseRank

  let private parseCh jAsJoker =
    function
    | 'A' -> Ace
    | 'K' -> King
    | 'Q' -> Queen
    | 'J' when jAsJoker -> Joker
    | 'J' -> Jack
    | 'T' -> Number 10
    | x -> Number(int x - int '0')

  let parse jAsJoker (s: string) : HandRank * Bid =
    let parts = s.Split ' '
    let parseCh' = parseCh jAsJoker

    match parts with
    | [| hand; bid |] ->
      let bid = int bid
      let chars = hand.ToCharArray() |> List.ofArray

      let rec inner acc chars =
        match chars with
        | x :: xs -> inner (acc @ [ parseCh' x ]) xs
        | [] -> acc

      let hand = inner [] chars

      rank hand, bid
    | _ -> failwithf "Invalid input: %s" s

let solve jAsJoker input =
  input
  |> Array.map (Parsing.parse jAsJoker)
  |> List.ofArray
  |> List.sortWith Ranking.sort 
  |> List.indexed
  |> List.fold (fun sum (i, hand) -> sum + (i + 1) * snd hand) 0

let partOne input =
  input |> solve false |> printfn "Part one: %A"

let partTwo input =
  input |> solve true |> printfn "Part two: %A"

let input = Input.readLines ()
partOne input 
partTwo input
