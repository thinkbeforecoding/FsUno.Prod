module Specifications


open FsUnit.Xunit
open Game
open PrettyPrint

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

