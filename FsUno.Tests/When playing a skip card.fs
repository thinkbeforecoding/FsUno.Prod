module FsUno.Tests.``When playing a skip card``

open FsUno.Domain
open Deck
open Game

open Specifications
open Xunit

let gameId = GameId 1

[<Fact>]
let ``It should skip the next player``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = green Three; NextPlayer = 1} ]
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = Skip Green } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = Skip Green; NextPlayer = 3} ]

[<Fact>]
let ``The next player cannot play``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = green Three; NextPlayer = 1}
            CardPlayed { GameId = gameId; Player = 1; Card = Skip Green; NextPlayer = 3} ]
    |> When ( PlayCard { GameId = gameId; Player = 2; Card = green Height } )
    |> Expect [ PlayerPlayedAtWrongTurn { GameId = gameId; Player = 2; Card = green Height } ]

[<Fact>]
let ``The second next player can play``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = green Three; NextPlayer = 1}
            CardPlayed { GameId = gameId; Player = 1; Card = Skip Green; NextPlayer = 3} ]
    |> When ( PlayCard { GameId = gameId; Player = 3; Card = green Height } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 3; Card = green Height; NextPlayer = 0 } ]

[<Fact>]
let ``skip should skip when counterclockwise``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } 
            CardPlayed { GameId = gameId; Player = 0; Card = green Three; NextPlayer = 1}
            CardPlayed { GameId = gameId; Player = 1; Card = green Height; NextPlayer = 2}
            CardPlayed { GameId = gameId; Player = 2; Card = KickBack Green; NextPlayer = 1}
            DirectionChanged { GameId = gameId; Direction = CounterClockWise }]
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = Skip Green })
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = Skip Green; NextPlayer = 3}]
