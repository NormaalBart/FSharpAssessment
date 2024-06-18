﻿namespace Services

open Models
open Database.InstoreDatabase
open Thoth.Json.Net

type CandidateService(database: DatabaseInterface.IDatabase) =

    member this.GetAllCandidates() : Result<seq<Candidate>, ServiceError> =
        Ok (
            database.GetCandidates
        )

    member this.GetAllCandidatesByGuardian(guardianId: GuardianId) : Result<seq<Candidate>, ServiceError> =
        match this.GetAllCandidates() with
        | Ok candidates ->
            let filteredCandidates =
                candidates
                |> Seq.filter (fun candidate -> candidate.GuardianId = guardianId)
            Ok filteredCandidates
        | Error error -> Error error

    member this.GetCandidate(name: string) : Result<Candidate, ServiceError> =
        match database.GetCandidate name with
        | Some candidate -> Ok(candidate)
        | None -> Error (ServiceError.NotFound "No candidate found")

    member this.AddCandidate(candidate: Candidate) : Result<Candidate, ServiceError> =
        match database.InsertCandidate candidate with
        | Ok () -> Ok (candidate)
        | Error (UniquenessError msg) -> Error (ServiceError.UniquenessError msg)

    member this.UpdateCandidate(candidate: Candidate) : Result<Candidate, ServiceError> =
        Ok (database.UpdateCandidate candidate)

    member this.DecodeCandidate(json: string) : Result<Candidate, ServiceError> =
        match Decode.fromString Candidate.decode json with
        | Ok (Ok candidate) -> Ok candidate
        | Ok (Error err) -> 
            let errorMessage =
                match err with
                | Candidate.InvalidName _ -> "Invalid name"
                | Candidate.InvalidGuardianId _ -> "Invalid guardian ID"
                | Candidate.InvalidDiploma _ -> "Invalid diploma"
                | Candidate.MissingField message -> "missing field " + message
                | Candidate.UpgradeNotAllowed _ -> "Upgrade not allowed"
            Error (ServiceError.InvalidData errorMessage)
        | Error err -> Error (ServiceError.InvalidData err)