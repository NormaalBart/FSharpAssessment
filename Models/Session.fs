namespace Models

open Thoth.Json.Net
open Thoth.Json.Giraffe
open System

type PoolType = 
    | Deep
    | Shallow

type Minutes = private Minutes of int

module Minutes =

    type Error =
        | InvalidMinutes of string

    let create (minutes: int) : Result<Minutes, Error> =
        if minutes >= 0 && minutes <= 30 then
            Ok (Minutes minutes)
        else
            Error (InvalidMinutes "Minutes must be between 0 and 30")

    let value (Minutes minutes) = minutes

type Session =
    { Pool: PoolType
      Date: DateTime
      Minutes: Minutes }

module Session =

    type Error =
        | InvalidMinutes of Minutes.Error
        | InvalidPoolType of string

    let build (pool: PoolType) (date: DateTime) (minutes: int) : Result<Session, Error> =
        match Minutes.create minutes with
        | Ok m -> Ok { Pool = pool; Date = date; Minutes = m }
        | Error e -> Error (InvalidMinutes e)

    let encode (session: Session) : JsonValue =
        Encode.object
            [ "pool", Encode.string (match session.Pool with | Deep -> "deep" | Shallow -> "shallow")
              "date", Encode.datetime session.Date
              "minutes", Encode.int (Minutes.value session.Minutes) ]

    let decode : Decoder<Result<Session, Error>> =
        Decode.object (fun get ->
            let poolType = get.Required.Field "pool" Decode.string
            let date = get.Required.Field "date" Decode.datetime
            let minutes = get.Required.Field "minutes" Decode.int
            match poolType with
            | "deep" -> build Deep date minutes
            | "shallow" -> build Shallow date minutes
            | _ -> Error (InvalidPoolType "Invalid pool type"))
