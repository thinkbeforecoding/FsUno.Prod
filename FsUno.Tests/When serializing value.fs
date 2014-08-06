module ``When serializing value``

open Xunit
open FsUnit.Xunit
open Game
open Serialization
open Newtonsoft.Json
open System.IO

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
    let converters = [ valueConverter typeof<GameId> ]

    GameId 1234
    |> serialize converters
    |> should equal "1234"

[<Fact>]
let ``a value should be deserialized from its content``() =
    let converters = [ valueConverter typeof<GameId> ]
    
    "1234"
    |> deserialize<GameId> converters
    |> should equal (GameId 1234)
