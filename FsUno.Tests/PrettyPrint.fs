module PrettyPrint

open Deck
open Game

let printCard =
    function
    | Digit(n, color) -> sprintf "%A %O" color n
    | KickBack(color) -> sprintf "%A Kickback" color
    
let cardPrinter () = printCard

let printEvent =
    function
    | GameStarted e -> sprintf "Game %O started with %d players. Top Card is %a" e.GameId e.PlayerCount cardPrinter e.FirstCard
    | CardPlayed e -> sprintf "Player %d played %a" e.Player cardPrinter e.Card
    | PlayerPlayedAtWrongTurn e -> sprintf "Player %d playerd at wrong turn a %a" e.Player cardPrinter e.Card 
    | DirectionChanged e -> sprintf "Game direction changed to %A" e.Direction

let printCommand  =
    function
    | StartGame c -> sprintf "Start game %O with %d players. Top card %a" c.GameId c.PlayerCount cardPrinter c.FirstCard
    | PlayCard c -> sprintf "Player %d plays %a" c.Player cardPrinter c.Card

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