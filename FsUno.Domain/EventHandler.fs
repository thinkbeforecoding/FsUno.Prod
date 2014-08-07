namespace FsUno.Domain

open Deck
open Game

open System

type EventHandler() =
    let mutable turnCount = 0

    let setColor card =
        let color = 
            match card with
            | Digit(_, c) -> Some c
            | KickBack c -> Some c
            |> function
               | Some Red -> ConsoleColor.Red
               | Some Green -> ConsoleColor.Green
               | Some Blue -> ConsoleColor.Blue
               | Some Yellow -> ConsoleColor.Yellow
               | None -> ConsoleColor.White
        Console.ForegroundColor <- color

    
    let printCard = function
        | Digit(n,c) -> sprintf "%A %d" c n.value 
        | KickBack c -> sprintf "%A kickback" c

    let printer f (w:IO.TextWriter) v = w.Write(f v : string)
    let cardPrinter = printer printCard

    member this.Handle = function
        | GameStarted event ->
            printfn "Game %O started with %d players" event.GameId event.PlayerCount
            setColor event.FirstCard
            printfn "First card: %A" event.FirstCard
        | CardPlayed event ->
            turnCount <- turnCount + 1
            setColor event.Card
            printfn "[%d] Player %d played %A" turnCount event.Player event.Card
        | PlayerPlayedAtWrongTurn event ->
            Console.ForegroundColor <- ConsoleColor.DarkRed
            printfn "[%d] Player %d played at wrong turn a %a" turnCount event.Player cardPrinter event.Card
        | DirectionChanged event ->
            printfn "     Direction changed. Playing now %A" event.Direction