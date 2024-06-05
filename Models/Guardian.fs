namespace Models

open Thoth.Json.Net
open Models

type Guardian =
    { Id: GuardianId
      Name: Name
      Candidates: List<Candidate> }

module Guardian =

    type Error =
        | InvalidId of GuardianId.Error
        | InvalidName of Name.Error
        | CandidateError of Candidate.Error list

    let build (id: string) (name: string) (candidates: List<Candidate>) : Result<Guardian, Error> =
        match GuardianId.create id, Name.create name with
        | Ok i, Ok n -> Ok { Id = i; Name = n; Candidates = candidates }
        | Error e, _ -> Error (InvalidId e)
        | _, Error e -> Error (InvalidName e)

    let encode (guardian: Guardian) : JsonValue =
        Encode.object
            [ "id", Encode.string (GuardianId.value guardian.Id)
              "name", Encode.string (Name.value guardian.Name)
              "candidates", Encode.list (guardian.Candidates |> List.map Candidate.encode) ]

    let decode : Decoder<Result<Guardian, Error>> =
        Decode.object (fun get ->
            let id = get.Required.Field "id" Decode.string
            let name = get.Required.Field "name" Decode.string
            let candidatesResult = get.Required.Field "candidates" (Decode.list Candidate.decode)
            match candidatesResult with
            | candidates when candidates |> List.forall Result.isOk ->
                let validCandidates = candidates |> List.choose (function | Ok c -> Some c | _ -> None)
                build id name validCandidates
            | candidates ->
                let errors = candidates |> List.choose (function | Error e -> Some e | _ -> None)
                Error (CandidateError errors))
