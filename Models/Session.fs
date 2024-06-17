namespace Models

open Thoth.Json.Net
open System

type PoolType = 
    | Deep
    | Shallow

module PoolType = 
    
    type Error =
        | InvalidPoolType of string

    let create (poolType: string) : Result<PoolType, Error> =
        match Util.makeUpperCase poolType with
            | "DEEP" -> Ok PoolType.Deep
            | "SHALLOW" -> Ok PoolType.Shallow
            | _ -> Error (InvalidPoolType "Invalid pool type")

    let value (poolType: PoolType) = 
        match poolType with
            | Deep -> "DEEP"
            | Shallow -> "SHALLOW"

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
        | InvalidPoolType of PoolType.Error
        | MissingField of string

    let build (pool: string) (date: DateTime) (minutes: int) : Result<Session, Error> =
        match PoolType.create pool, Minutes.create minutes with
        | Ok poolType, Ok minutes  -> Ok { Pool = poolType; Date = date; Minutes = minutes }
        | Error e, _ -> Error(InvalidPoolType e)
        | _, Error e -> Error (InvalidMinutes e)

    let isApplicableForDiploma (diploma: Diploma.Diploma) (session: Session) : bool =
        match diploma with
        | Diploma.None -> true
        | Diploma.A -> (session.Pool = Shallow || session.Pool = Deep) && (Minutes.value session.Minutes >= 1)
        | Diploma.B -> session.Pool = Deep && (Minutes.value session.Minutes >= 10)
        | Diploma.C -> session.Pool = Deep && (Minutes.value session.Minutes >= 15)

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
            | Some p, Some d, Some m -> build p d m
            | None, _, _ -> Error (MissingField "pool")
            | _, None, _ -> Error (MissingField "date")
            | _, _, None -> Error (MissingField "minutes"))