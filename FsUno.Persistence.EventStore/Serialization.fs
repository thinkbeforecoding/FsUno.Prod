module Serialization

open FsUno.Domain

// This module provides Json serialization to store
// events in the event store 

// It is based on Json.net but provides
// a specialization for cleaner F# type serialization

open System
open System.Reflection
open Newtonsoft.Json
open Microsoft.FSharp.Reflection

// Basic reflection for converters
module private Reflection = 
    let isGeneric td (t:Type) = 
        t.IsGenericType && t.GetGenericTypeDefinition() = td
        
    let isList t = isGeneric typedefof<List<_>> t
    let isOption t = isGeneric typedefof<Option<_>> t

    let propertyName (case: PropertyInfo) = case.Name

    let (|NamedCase|UnionCase|SingleCase|) v =            
        let t = v.GetType()
        match FSharpValue.GetUnionFields(v, t) with
        | case, [||] -> NamedCase case.Name
        | case, values -> 
            let names =
                case.GetFields() 
                |> Seq.map propertyName
            let values = Seq.zip names values |> Seq.toList 
            match values with
            | [ value ] when FSharpType.GetUnionCases(t).Length = 1 ->
                SingleCase(case.Name, value)
            | _ -> UnionCase(case.Name, values)

    let getCase t caseName =
            FSharpType.GetUnionCases(t)
            |> Array.find (fun c -> c.Name = caseName)
    let getFields (case: UnionCaseInfo) =
        case.GetFields()
        |> Array.mapi (fun i c -> c.Name, (i,c.PropertyType))
        |> Map.ofArray

    /// Returns all value type containing a single property from
    /// specified assembly
    let getValues (assembly: Assembly) =
        let isStruct (t: Type) = t.IsValueType
        let hasOneProperty (t: Type) = Array.length (t.GetProperties()) = 1
        assembly.GetTypes()
        |> Seq.filter isStruct
        |> Seq.filter hasOneProperty


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

    let deserializeUnion (r: JsonReader) (s:JsonSerializer) (t: Type) getCase =
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
            match FSharpType.GetUnionCases t with
            | [| case |] when case.GetFields().Length = 1 ->
                FSharpValue.MakeUnion(case, [| s.Deserialize(r, case.GetFields().[0].PropertyType) |])
            | _ ->
                let case = getCase r false
                FSharpValue.MakeUnion(case, null)

open Reflection

// This converter reads/writes a discriminated union
// as a record, adding a "_Case" field.
let unionConverter =
    { new JsonConverter() with
        member __.WriteJson(w,v,s) =
            match v with
            | NamedCase name -> w.WriteValue name
            | SingleCase(_, (_,fieldValue)) -> s.Serialize(w,fieldValue)
            | UnionCase(name, fields)  ->
                ("_Case", box name) :: fields
                |> Json.writeObject w s

        member __.ReadJson(r,t,_,s) =
            Json.deserializeUnion r s t (fun r s -> Json.readCaseName r s |> Reflection.getCase t)

        member __.CanConvert t =
            FSharpType.IsUnion t && not (isList t || isOption t) }

// This converter reads/writes a discriminated union
// but doesn't serialize the case. It is intended to be
// stored in the EventType of the event store.
let private rootUnionConverter<'a> (case: UnionCaseInfo) =
    { new JsonConverter() with
        member __.WriteJson(w,v,s) =
            match v with
            | NamedCase _ -> ()
            | SingleCase(_, (_,value)) -> s.Serialize(w, value)           
            | UnionCase(_, fields) ->
                fields
                |> Json.writeObject w s

        member __.ReadJson(r,t,_,s) =
            Json.deserializeUnion r s t (fun _ _ -> case)

        member __.CanConvert t =
            t = typeof<'a> || t.BaseType = typeof<'a> }

// This converter writes options as value or null
let private optionConverter =
    { new JsonConverter() with
        member __.WriteJson(w,v,s) = 
            match FSharpValue.GetUnionFields(v,v.GetType()) with
            | _, [|v|] -> s.Serialize(w, v)
            | _ -> w.WriteNull()

        member __.ReadJson(r,t,_,s) =
            let optionType =
                match t.GetGenericArguments() with
                | [|o|] -> o
                | _ -> failwith "Option should have a single generic argument"
            let cases = FSharpType.GetUnionCases(t)
            
            if r.TokenType = JsonToken.Null then
                FSharpValue.MakeUnion(cases.[0], null)
            else
                FSharpValue.MakeUnion(cases.[1], [| s.Deserialize(r,optionType) |])
                                
        member __.CanConvert t = isOption t }

/// Serializes a value type containing a single property
/// as its inner value. It is used for digit and GameId
let valueConverter (valueType: Type) =
    let field =
        match valueType.GetProperties() with
        | [| p |] -> p
        | _ -> invalidArg "valueType" "The type passed to valueConverter should have a single property"
    let innerType = field.PropertyType
    let ctor = valueType.GetConstructor [| innerType |]
    { new JsonConverter() with
        member __.WriteJson(w,v,s) =
            s.Serialize(w, field.GetValue(v))
        member __.ReadJson(r,_,_,s) =
            ctor.Invoke([| s.Deserialize(r, innerType) |])
        member __.CanConvert t = t = valueType }

let converters =
    let valueConverters =
        getValues typeof<Game.GameId>.Assembly
        |> Seq.map valueConverter
        |> Seq.toList
    [ unionConverter;optionConverter]
    @ valueConverters

let deserializeUnion<'a> eventType data = 
    FSharpType.GetUnionCases(typeof<'a>)
    |> Array.tryFind (fun c -> c.Name = eventType)
    |> function
       | Some case ->  
            let serializer = new JsonSerializer()
            rootUnionConverter<'a> case :: converters
            |> List.iter serializer.Converters.Add
            
            use stream = new IO.MemoryStream(data: byte[])
            use reader = new JsonTextReader(new IO.StreamReader(stream))
            serializer.Deserialize<'a>(reader)
            |> Some
       | None -> None


let serializeUnion (o:'a)  =
    let case,_ = FSharpValue.GetUnionFields(o, typeof<'a>)
    let serializer = new JsonSerializer()
    rootUnionConverter<'a> case :: converters
    |> List.iter serializer.Converters.Add
    use stream = new IO.MemoryStream()
    use writer = new IO.StreamWriter(stream)
    serializer.Serialize(writer, o)
    writer.Flush()
    case.Name, stream.ToArray()