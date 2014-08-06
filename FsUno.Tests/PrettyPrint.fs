module PrettyPrint

open Deck
open Game

let printCard =
    function
    | Digit(n, color) -> sprintf "%A %d" color n
    | KickBack(color) -> sprintf "%A Kickback" color
    
let printEvent =
    function
    | GameStarted e -> sprintf "Game %d started with %d players. Top Card is %s" e.GameId e.PlayerCount (printCard e.FirstCard)
    | CardPlayed e -> sprintf "Player %d played %s" e.Player (printCard e.Card)

let printCommand  =
    function
    | StartGame c -> sprintf "Start game %d with %d players. Top card %s" c.GameId c.PlayerCount (printCard c.FirstCard)
    | PlayCard c -> sprintf "Player %d plays %s" c.Player (printCard c.Card)

let printGiven events =
    printfn "Given"
    events 
    |> List.map printEvent
    |> List.iter (printfn "\t%s")
   
let printWhen command =
    printfn "When"
    command |> printCommand  |> printfn "\t%s"

let printExpect events =
    printfn "Expect"
    events 
    |> List.map printEvent
    |> List.iter (printfn "\t%s")

let printExpectThrows ex =
    printfn "Expect"
    printfn "\t%A" ex    