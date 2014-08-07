module FsUno.Tests.Specifications

open FsUno.Domain
open Game

open PrettyPrint
open FsUnit.Xunit

let Given (events: Event list) = events
let When (command: Command) events = events, command
let Expect (expected: Event list) (events, command) =
    printGiven events
    printWhen command
    printExpect expected

    replay events
    |> handle command
    |> should equal expected

let ExpectThrows<'Ex> (events, command) =
    printGiven events
    printWhen command
    printExpectThrows typeof<'Ex>


    (fun () ->
        replay events
        |> handle command
        |> ignore)
    |> should throw typeof<'Ex>