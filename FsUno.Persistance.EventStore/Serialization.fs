module Serialization

// This module provides Json serialization to store
// events in the event store 

// It is based on Json.net but provides
// a specialization for cleaner F# type serialization

open System
open System.Reflection
open System.IO
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Microsoft.FSharp.Reflection

// Basic reflection for converters
module private Reflection = 
    let isGeneric td (t:Type) = 
        t.IsGenericType && t.GetGenericTypeDefinition() = td
        
    let isList t = isGeneric typedefof<List<_>> t
    let isOption t = isGeneric typedefof<Option<_>> t

    let propertyName (case: PropertyInfo) = case.Name

    type UnionValue =
        | NamedCase of string
        | ValueCase of string * (string * obj) list

    let unionValues v=            
        match FSharpValue.GetUnionFields(v, v.GetType()) with
        | case, [||] -> NamedCase case.Name
        | case, values -> 
            let names =
                case.GetFields() 
                |> Seq.map propertyName
            ValueCase(case.Name, Seq.zip names values |> Seq.toList)

    let getCase t caseName =
            FSharpType.GetUnionCases(t)
            |> Array.find (fun c -> c.Name = caseName)
    let getFields (case: UnionCaseInfo) =
        case.GetFields()
        |> Array.mapi (fun i c -> c.Name, (i,c.PropertyType))
        |> Map.ofArray

// Json function used by converters
module private Json =    
    let writeObject (w: JsonWriter) (s: JsonSerializer) properties =
        let writeProperty (name, value) = 
            w.WritePropertyName(name)
            s.Serialize(w, value)
        w.WriteStartObject()
        List.iter writeProperty properties
        w.WriteEndObject() 
    
    let read (r: JsonReader) = r.Read() |> ignore

    let deserializeField (r: JsonReader) (s:JsonSerializer) case =
        let fieldMap = Reflection.getFields case
        fun () ->
            let fieldName = string r.Value
            read r
            let i, fieldType = Map.find fieldName fieldMap
            let prop = i, s.Deserialize(r, fieldType)
            read r
            prop

    let readCaseName (r: JsonReader) shouldSkip =
        if r.TokenType = JsonToken.PropertyName then
            read r

        let name = string r.Value
        if shouldSkip then
            read r
        name

    let deserializeUnion (r: JsonReader) (s:JsonSerializer) getCase =

        if r.TokenType = JsonToken.StartObject then
            read r
            let case = getCase r true

            
            let deserializeField = case |> deserializeField r s

            let rec loop values =
                if r.TokenType = JsonToken.EndObject then
                    values
                else
                    let fieldValue = deserializeField()
                    loop (fieldValue :: values)

            let values =
                loop []
                |> Seq.sortBy fst
                |> Seq.map snd
                |> Seq.toArray

            FSharpValue.MakeUnion(case,values)
        else
            let case = getCase r false
            FSharpValue.MakeUnion(case, null)

open Reflection

// This converter reads/writes a discriminated union
// as a record, adding a "_Case" field.
let private unionConverter =
    { new JsonConverter() with
        member this.WriteJson(w,v,s) =
            match unionValues v with
            | NamedCase name -> w.WriteValue name
            | ValueCase(name, fields) ->
                ("_Case", box name) :: fields
                |> Json.writeObject w s

        member this.ReadJson(r,t,v,s) =
            Json.deserializeUnion r s (fun r s -> Json.readCaseName r s |> Reflection.getCase t)

        member this.CanConvert t =
            FSharpType.IsUnion t && not (isList t || isOption t) }

// This converter reads/writes a discriminated union
// but doesn't serialize the case. It is intended to be
// stored in the EventType of the event store.
let private rootUnionConverter<'a> (case: UnionCaseInfo) =
    { new JsonConverter() with
        member this.WriteJson(w,v,s) =
            match unionValues v with
            | NamedCase _ -> ()
            | ValueCase(_, fields) ->
                fields
                |> Json.writeObject w s

        member this.ReadJson(r,t,v,s) =
            Json.deserializeUnion r s (fun _ _ -> case)

        member this.CanConvert t =
            t = typeof<'a> || t.BaseType = typeof<'a> }

// This converter writes options as value or null
let private optionConverter =
    { new JsonConverter() with
        member this.WriteJson(w,v,s) = 
            match FSharpValue.GetUnionFields(v,v.GetType()) with
            | _, [|v|] -> s.Serialize(w, v)
            | _ -> w.WriteNull()

        member this.ReadJson(r,t,v,s) =
            let optionType =
                match t.GetGenericArguments() with
                | [|o|] -> o
                | _ -> failwith "Option should have a single generic argument"
            let cases = FSharpType.GetUnionCases(t)
            
            if r.TokenType = JsonToken.Null then
                FSharpValue.MakeUnion(cases.[0], null)
            else
                FSharpValue.MakeUnion(cases.[1], [| s.Deserialize(r,optionType) |])
                                
        member this.CanConvert t = isOption t }

let deserializeUnion<'a> eventType data = 
    FSharpType.GetUnionCases(typeof<'a>)
    |> Array.tryFind (fun c -> c.Name = eventType)
    |> function
       | Some case ->  
            let serializer = new JsonSerializer()
            [rootUnionConverter<'a> case; unionConverter;optionConverter ]
            |> List.iter serializer.Converters.Add
            use stream = new IO.MemoryStream(data: byte[])
            use reader = new JsonTextReader(new IO.StreamReader(stream))
            serializer.Deserialize<'a>(reader)
            |> Some
       | None -> None

let serializeUnion (o:'a)  =
    let case,_ = FSharpValue.GetUnionFields(o, typeof<'a>)
    let serializer = new JsonSerializer()
    [rootUnionConverter<'a> case; unionConverter;optionConverter ]
    |> List.iter serializer.Converters.Add
    use stream = new IO.MemoryStream()
    use writer = new IO.StreamWriter(stream)
    serializer.Serialize(writer, o)
    writer.Flush()
    case.Name, stream.ToArray()


