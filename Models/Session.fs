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
        | MissingField of string

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
            let poolType = get.Optional.Field "pool" Decode.string
            let date = get.Optional.Field "date" Decode.datetime
            let minutes = get.Optional.Field "minutes" Decode.int
            match poolType, date, minutes with
            | Some p, Some d, Some m ->
                match p with
                | "deep" -> build Deep d m
                | "shallow" -> build Shallow d m
                | _ -> Error (InvalidPoolType "Invalid pool type")
            | None, _, _ -> Error (MissingField "pool")
            | _, None, _ -> Error (MissingField "date")
            | _, _, None -> Error (MissingField "minutes"))