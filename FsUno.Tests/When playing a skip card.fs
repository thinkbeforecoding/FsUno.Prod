module ``When playing a skip card``

open Xunit
open FsUnit.Xunit
open Specifications
open Deck
open Game

let gameId = GameId 1

[<Fact>]
let ``It should skip the next player``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } 
            CardPlayed { GameId = gameId; Player = 0; Card = green 3; NextPlayer = 1} ]
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = Skip Green } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = Skip Green; NextPlayer = 3} ]

[<Fact>]
let ``The next player cannot play``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } 
            CardPlayed { GameId = gameId; Player = 0; Card = green 3; NextPlayer = 1}
            CardPlayed { GameId = gameId; Player = 1; Card = Skip Green; NextPlayer = 3} ]
    |> When ( PlayCard { GameId = gameId; Player = 2; Card = green 8 } )
    |> Expect [ PlayerPlayedAtWrongTurn { GameId = gameId; Player = 2; Card = green 8 } ]

[<Fact>]
let ``The second next player can play``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } 
            CardPlayed { GameId = gameId; Player = 0; Card = green 3; NextPlayer = 1}
            CardPlayed { GameId = gameId; Player = 1; Card = Skip Green; NextPlayer = 3} ]
    |> When ( PlayCard { GameId = gameId; Player = 3; Card = green 8 } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 3; Card = green 8; NextPlayer = 0 } ]

[<Fact>]
let ``skip should skip when counterclockwise``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3} 
            CardPlayed { GameId = gameId; Player = 0; Card = green 3; NextPlayer = 1}
            CardPlayed { GameId = gameId; Player = 1; Card = green 8; NextPlayer = 2}
            CardPlayed { GameId = gameId; Player = 2; Card = KickBack Green; NextPlayer = 1}
            DirectionChanged { GameId = gameId; Direction = CounterClockWise }]
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = Skip Green })
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = Skip Green; NextPlayer = 3}]
