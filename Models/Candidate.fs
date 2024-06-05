namespace Models

open Thoth.Json.Net
open System

type Diploma = private Diploma of string

module Diploma =

    type Error =
        | InvalidDiploma of string

    let create (diploma: string) : Result<Diploma, Error> =
        match diploma with
        | "" | "A" | "B" | "C" -> Ok (Diploma diploma)
        | _ -> Error (InvalidDiploma "Diploma must be one of '', 'A', 'B', or 'C'")

    let value (Diploma diploma) = diploma

type Candidate =
    { Name: Name
      DateOfBirth: DateTime
      GuardianId: GuardianId
      Diploma: Diploma }

module Candidate =

    type Error =
        | InvalidName of Name.Error
        | InvalidGuardianId of GuardianId.Error
        | InvalidDiploma of Diploma.Error

    let build (name: string) (dateOfBirth: DateTime)  (guardianId: string) (diploma: string): Result<Candidate, Error> =
        match Name.create name, GuardianId.create guardianId, Diploma.create diploma with
        | Ok n, Ok g, Ok d -> Ok { Name = n; GuardianId = g; Diploma = d; DateOfBirth = dateOfBirth }
        | Error e, _, _ -> Error (InvalidName e)
        | _, Error e, _ -> Error (InvalidGuardianId e)
        | _, _, Error e -> Error (InvalidDiploma e)

    let encode (candidate: Candidate) : JsonValue =
        Encode.object
            [ "name", Encode.string (Name.value candidate.Name)
              "date_of_birth", Encode.datetime candidate.DateOfBirth
              "guardian_id", Encode.string (GuardianId.value candidate.GuardianId)
              "diploma", Encode.string (Diploma.value candidate.Diploma) ]

    let decode : Decoder<Result<Candidate, Error>> =
        Decode.object (fun get ->
            let name = get.Required.Field "name" Decode.string
            let dateOfBirth = get.Required.Field "date_of_birth" Decode.datetime
            let guardianId = get.Required.Field "guardian_id" Decode.string
            let diploma = ""
            match build name dateOfBirth guardianId diploma  with
            | Ok candidate -> Ok candidate
            | Error e -> Error e)

