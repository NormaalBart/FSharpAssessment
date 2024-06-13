namespace Services

open Models
open Database.InstoreDatabase
open Database.SeedStore
open Thoth.Json.Net

type CandidateService(store: Store) =

    member this.GetAllCandidates() : Result<seq<Candidate>, ServiceError> =
        Ok (
            InMemoryDatabase.all store.candidates
            |> Seq.choose (fun (name, bDay, gId, dpl) ->
            match Candidate.build name bDay gId dpl with
                | Ok candidate -> Some candidate
                | Error _ -> None)
        )

    member this.GetCandidate(name: string) : Result<Candidate, ServiceError> =
        match InMemoryDatabase.lookup name store.candidates with
        | None -> Error (ServiceError.NotFound "Candidate not found")
        | Some (name, bDay, gId, dpl) ->
            match Candidate.build name bDay gId dpl with
            | Ok candidate -> Ok candidate
            | Error _ -> Error (ServiceError.InvalidData "Invalid candidate data")

    member this.AddCandidate(candidate: Candidate) : Result<Candidate, ServiceError> =
        match InMemoryDatabase.insert (Name.value candidate.Name) (Name.value candidate.Name, candidate.DateOfBirth, GuardianId.value candidate.GuardianId, Diploma.value candidate.Diploma) store.candidates with
        | Ok () -> Ok (candidate)
        | Error (UniquenessError msg) -> Error (ServiceError.UniquenessError msg)

    member this.UpdateCandidate(candidate: Candidate) : Result<Candidate, ServiceError> =
        let candidateKey = Name.value candidate.Name
        let candidateData = (candidateKey, candidate.DateOfBirth, GuardianId.value candidate.GuardianId, Diploma.value candidate.Diploma)
        match InMemoryDatabase.update candidateKey candidateData store.candidates with
        | _ -> Ok(candidate)

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
            Error (ServiceError.InvalidData errorMessage)
        | Error err -> Error (ServiceError.InvalidData err)