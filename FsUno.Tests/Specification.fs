module FsUno.Tests.Specifications

open FsUno.Domain
open Game

open PrettyPrint
open FsUnit.Xunit

// A generic fold function that can be used on any aggregate
let inline fold events =
    let initial = (^S: (static member initial: ^S) ()) 
    let evolve s = (^S: (static member evolve: ^S -> (^E -> ^S)) s)
    List.fold evolve initial events

let Given (events: Event list) = events
let When (command: Command) events = events, command
let Expect (expected: Event list) (events, command) =
    printGiven events
    printWhen command
    printExpect expected

    fold events
    |> handle command
    |> should equal expected

let ExpectThrows<'Ex> (events, command) =
    printGiven events
    printWhen command
    printExpectThrows typeof<'Ex>


    (fun () ->
        fold events
        |> handle command
        |> ignore)
    |> should throw typeof<'Ex>