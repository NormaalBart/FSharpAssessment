namespace Services

open Models
open Database.InstoreDatabase
open Database.SeedStore

type CandidateService(store: Store) =

    member this.GetAllCandidates() : Result<seq<Candidate>, ServiceError> =
        Ok (
            InMemoryDatabase.all store.candidates
            |> Seq.choose (fun (name, _, gId, dpl) ->
                match Candidate.build name gId dpl with
                | Ok candidate -> Some candidate
                | Error _ -> None)
        )

    member this.GetCandidate(name: string) : Result<Candidate, ServiceError> =
        match InMemoryDatabase.lookup name store.candidates with
        | None -> Error (ServiceError.NotFound "Candidate not found")
        | Some (name, _, gId, dpl) ->
            match Candidate.build name gId dpl with
            | Ok candidate -> Ok candidate
            | Error _ -> Error (ServiceError.InvalidData "Invalid candidate data")
