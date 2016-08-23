module FsUno.Tests.``When serializing value``

open FsUno.Domain
open Deck
open Game

open Serialization

open FsUnit.Xunit
open Newtonsoft.Json
open System.IO
open Xunit

let createSerializer converters =
    let serializer = JsonSerializer()
    converters |> List.iter serializer.Converters.Add
    serializer

let serialize converters o = 
    let serializer = createSerializer converters
    use w = new StringWriter()
    serializer.Serialize(w, o)
    w.ToString()

let deserialize<'a> converters s =
    let serializer = createSerializer converters
    use r = new StringReader(s)
    serializer.Deserialize<'a>(new JsonTextReader(r))

[<Fact>]
let ``a value should be serialized as its content``() =
    let converters = [ unionConverter ]
    let result =
        Seven 
        |> serialize converters
    result = "\"Seven\"" |> should be True

[<Fact>]
let ``a value should be deserialized from its content``() =
    let converters = [ unionConverter ]
    
    "\"Seven\""
    |> deserialize<Digit> converters
    |> should equal Seven

[<Fact>]
let ``a single case union should be serialized as its content``() =
    let converters = [ unionConverter ]

    GameId 1234
    |> serialize converters
    |> should equal "1234"

[<Fact>]
let ``a single case union should be deserialized from its content``() =
    let converters = [ unionConverter ]
    
    "1234"
    |> deserialize<GameId> converters
    |> should equal (GameId 1234)