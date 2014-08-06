module FsUno.Tests.``When playing a second turn``

open Xunit
open System
open Specifications
open Deck
open Game

let gameId = GameId 1

[<Fact>]
let ``Same color should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = red 8 } )                  
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = red 8 } ]

[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = yellow 9 } )                  
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = yellow 9 } ]

[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = yellow 8 } )                  
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``First player should play at his turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 2; Card = green 9 } )                
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``After a full round it should be player 0 turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9 }
            CardPlayed { GameId = gameId; Player = 1; Card = red 8 }
            CardPlayed { GameId = gameId; Player = 2; Card = red 6 }
            CardPlayed { GameId = gameId; Player = 3; Card = red 6 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = red 1 } )                
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = red 1 } ]
