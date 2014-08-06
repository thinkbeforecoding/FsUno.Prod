module FsUno.Tests.``When playing a second turn``

open Xunit
open System
open Specifications
open Deck
open Game


[<Fact>]
let ``Same color should be accepted``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red) } 
            CardPlayed { GameId = 1; Player = 0; Card = Digit(9, Red) } ] 
    |> When ( PlayCard { GameId = 1; Player = 1; Card = Digit(8, Red) } )                  
    |> Expect [ CardPlayed { GameId = 1; Player = 1; Card = Digit(8, Red) } ]

[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red) } 
            CardPlayed { GameId = 1; Player = 0; Card = Digit(9, Red) } ] 
    |> When ( PlayCard { GameId = 1; Player = 1; Card = Digit(9, Yellow) } )                  
    |> Expect [ CardPlayed { GameId = 1; Player = 1; Card = Digit(9, Yellow) } ]

[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red) } 
            CardPlayed { GameId = 1; Player = 0; Card = Digit(9, Red) } ] 
    |> When ( PlayCard { GameId = 1; Player = 1; Card = Digit(8, Yellow) } )                  
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``First player should play at his turn``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red) } 
            CardPlayed { GameId = 1; Player = 0; Card = Digit(9, Red) } ] 
    |> When ( PlayCard { GameId = 1; Player = 2; Card = Digit(9, Green) } )                
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``After a full round it should be player 0 turn``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = Digit(3, Red) } 
            CardPlayed { GameId = 1; Player = 0; Card = Digit(9, Red) }
            CardPlayed { GameId = 1; Player = 1; Card = Digit(8, Red) }
            CardPlayed { GameId = 1; Player = 2; Card = Digit(6, Red) }
            CardPlayed { GameId = 1; Player = 3; Card = Digit(6, Red) } ] 
    |> When ( PlayCard { GameId = 1; Player = 0; Card = Digit(1, Red) } )                
    |> Expect [ CardPlayed { GameId = 1; Player = 0; Card = Digit(1, Red) } ]
