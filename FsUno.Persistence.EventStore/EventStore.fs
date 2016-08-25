module EventStore

open FsUno.Domain.Game


open System
open System.Net
open EventStore.ClientAPI
open Serialization

type IEventStoreConnection with
    member this.AsyncConnect() = this.ConnectAsync() |> Async.AwaitTask
    member this.AsyncReadStreamEventsForward stream start count resolveLinkTos =
        this.ReadStreamEventsForwardAsync(stream, start, count, resolveLinkTos)
        |> Async.AwaitTask
    member this.AsyncAppendToStream stream expectedVersion events =
        this.AppendToStreamAsync(stream, expectedVersion, events)
        |> Async.AwaitTask


let deserialize (event: ResolvedEvent) = deserializeUnion event.Event.EventType event.Event.Data

let serialize event = 
    let typeName, data = serializeUnion event
    EventData(Guid.NewGuid(), typeName, true, data, null)

let create() = 
    async {
        let s = EventStoreConnection.Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113))
        do! Async.AwaitTask ( s.ConnectAsync() )
        return s }

let subscribe (projection: Event -> unit) (getStore: Async<IEventStoreConnection>) =
    async {
    let! store = getStore
    let credential = SystemData.UserCredentials("admin", "changeit")
    do! Async.AwaitTask
        <| store.SubscribeToAllAsync(true, (fun _ e -> deserialize e |> Option.iter projection), userCredentials =      credential) |> Async.Ignore
    return store }
    |> Async.RunSynchronously

let readStream (store: IEventStoreConnection) streamId version count = 
    async {
        let! slice = store.AsyncReadStreamEventsForward streamId version count true

        let events = 
            slice.Events 
            |> Seq.choose deserialize
            |> Seq.toList
        
        let nextEventNumber = 
            if slice.IsEndOfStream 
            then None 
            else Some slice.NextEventNumber

        return events, slice.LastEventNumber, nextEventNumber }

let appendToStream (store: IEventStoreConnection) streamId expectedVersion newEvents = 
    async {
        let serializedEvents = [| for event in newEvents -> serialize event |]

        do! Async.Ignore <| store.AsyncAppendToStream streamId expectedVersion serializedEvents }