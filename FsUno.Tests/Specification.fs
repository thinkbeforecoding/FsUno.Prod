module FsUno.Tests.Specifications

open FsUno.Domain
open Game

open PrettyPrint
open FsUnit.Xunit

// A generic replay function that can be used on any aggregate
let inline replay events =
    let initial = (^S: (static member initial: ^S) ()) 
    let apply s = (^S: (static member apply: ^S -> (^E -> ^S)) s)
    List.fold apply initial events

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