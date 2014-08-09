module FsUno.Tests.``When playing a second turn``

open FsUno.Domain
open Deck
open Game

open Specifications
open System
open Xunit

let gameId = GameId 1

[<Fact>]
let ``Same color should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = red 8 } )                  
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = red 8; NextPlayer = 2 } ]

[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = yellow 9 } )                  
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = yellow 9; NextPlayer = 2 } ]

[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = yellow 8 } )                  
    |> Expect [ PlayerPlayedWrongCard { GameId = gameId; Player = 1; Card = yellow 8}]

[<Fact>]
let ``Player should play at his turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 2; Card = green 9 } )                
    |> Expect [ PlayerPlayedAtWrongTurn { GameId = gameId; Player = 2; Card = green 9 }]

[<Fact>]
let ``After a full round it should be player 0 turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1 }
            CardPlayed { GameId = gameId; Player = 1; Card = red 8; NextPlayer = 2 }
            CardPlayed { GameId = gameId; Player = 2; Card = red 6; NextPlayer = 3 } ]
    |> When ( PlayCard { GameId = gameId; Player = 3; Card = red 1 } )                
    |> Expect [ CardPlayed { GameId = gameId; Player = 3; Card = red 1; NextPlayer = 0 } ]
