module FsUno.Tests.``When passing to next turn``

open FsUno.Domain
open Deck
open Game

open FsUnit.Xunit
open Xunit

[<Fact>]
let ``clockwise should be next one``() =
    let turn =
        Turn.start 0 4
        |> Turn.next 
    turn.Player |> should equal 1

[<Fact>]
let ``clockwise after last player should be first one``() =
    let turn =
        Turn.start 3 4
        |> Turn.next
    turn.Player |> should equal 0

[<Fact>]
let ``revesing turn should change order``() =
    let turn =
        Turn.start 1 4
        |> Turn.reverse
    turn.Direction |> should equal CounterClockWise 

[<Fact>]
let ``revesing twice should bring back clockwise order``() =
    let turn =
        Turn.start 1 4
        |> Turn.reverse
        |> Turn.reverse
    turn.Direction |> should equal ClockWise 

[<Fact>]
let ``counterclockwise should be previous one``() =
    let turn =
        Turn.start 0 4
        |> Turn.reverse
        |> Turn.next
    turn.Player|> should equal 3
