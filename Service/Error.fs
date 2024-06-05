namespace Services

open Thoth.Json.Net

type ServiceError =
    | NotFound of string
    | InvalidData of string
    | UniquenessError of string

module ServiceError =
    let encode (error: ServiceError) : JsonValue =
        match error with
        | NotFound msg -> 
            Encode.object [
                "type", Encode.string "NotFound"
                "message", Encode.string msg
            ]
        | InvalidData msg -> 
            Encode.object [
                "type", Encode.string "InvalidData"
                "message", Encode.string msg
            ]
        | UniquenessError msg -> 
            Encode.object [
                "type", Encode.string "UniquenessError"
                "message", Encode.string msg
            ]