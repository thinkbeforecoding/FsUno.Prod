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
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red Nine; NextPlayer = 1 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = red Eight } )                  
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = red Eight; NextPlayer = 2 } ]

[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red Nine; NextPlayer = 1 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = yellow Nine } )                  
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = yellow Nine; NextPlayer = 2 } ]

[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red Nine; NextPlayer = 1 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = yellow Eight } )                  
    |> Expect [ PlayerPlayedWrongCard { GameId = gameId; Player = 1; Card = yellow Eight}]

[<Fact>]
let ``Player should play at his turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red Nine; NextPlayer = 1 } ] 
    |> When ( PlayCard { GameId = gameId; Player = 2; Card = green Nine } )                
    |> Expect [ PlayerPlayedAtWrongTurn { GameId = gameId; Player = 2; Card = green Nine }]

[<Fact>]
let ``After a full round it should be player 0 turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = red Nine; NextPlayer = 1 }
            CardPlayed { GameId = gameId; Player = 1; Card = red Eight; NextPlayer = 2 }
            CardPlayed { GameId = gameId; Player = 2; Card = red Six; NextPlayer = 3 } ]
    |> When ( PlayCard { GameId = gameId; Player = 3; Card = red One} )                
    |> Expect [ CardPlayed { GameId = gameId; Player = 3; Card = red One; NextPlayer = 0 } ]
