open FsUno
open CommandHandlers


open Deck
open Game
open EventStore

[<EntryPoint>]
let main _ = 
    
    let eventHandler = new EventHandler()
    use store = 
        EventStore.create()
        |> subscribe eventHandler.Handle


    let handle = Game.create (readStream store) (appendToStream store)

    handle (StartGame { GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red)})
    
    handle (PlayCard { GameId = 1; Player = 0; Card = Digit(3, Blue) })

    handle (PlayCard { GameId = 1; Player = 1; Card = Digit(8, Blue) })
    
    handle (PlayCard { GameId = 1; Player = 2; Card = Digit(8, Yellow) })
    
    handle (PlayCard { GameId = 1; Player = 3; Card = Digit(4, Yellow) })
    
    handle (PlayCard { GameId = 1; Player = 0; Card = Digit(4, Green) })


    System.Console.ReadLine() |> ignore

    0 // return an integer exit code
