module EventStore

open System
open FsUno.Domain.Game

// This module implements AwaitTask for non generic Task
// It should be useless in F# 4 since it should be implemented in FSharp.Core
[<AutoOpen>]
module AsyncExtensions =
    open System
    open System.Threading
    open System.Threading.Tasks
    type Microsoft.FSharp.Control.Async with
        static member Raise(ex) = Async.FromContinuations(fun (_,econt,_) -> econt ex)

        static member AwaitTask (t: Task) =
            let tcs = new TaskCompletionSource<unit>(TaskContinuationOptions.None)
            t.ContinueWith((fun _ -> 
                if t.IsFaulted then tcs.SetException t.Exception
                elif t.IsCanceled then tcs.SetCanceled()
                else tcs.SetResult(())), TaskContinuationOptions.ExecuteSynchronously) |> ignore
            async {
                try
                    do! Async.AwaitTask tcs.Task
                with
                | :? AggregateException as ex -> 
                    do! Async.Raise (ex.Flatten().InnerExceptions |> Seq.head) }


// This module uses a production ready event store
// This version use the async API
open System
open System.Net
open EventStore.ClientAPI
open Serialization

type IEventStoreConnection with
    member this.AsyncConnect() = Async.AwaitTask(this.ConnectAsync())
    member this.AsyncReadStreamEventsForward stream start count resolveLinkTos =
        Async.AwaitTask(this.ReadStreamEventsForwardAsync(stream, start, count, resolveLinkTos))
    member this.AsyncAppendToStream stream expectedVersion events =
        Async.AwaitTask(this.AppendToStreamAsync(stream, expectedVersion, events))


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
        <| store.SubscribeToAllAsync(true, (fun s e -> deserialize e |> Option.iter projection), userCredentials =      credential) |> Async.Ignore
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