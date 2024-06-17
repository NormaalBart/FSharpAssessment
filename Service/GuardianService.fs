namespace Services

open Models
open Database.InstoreDatabase
open Thoth.Json.Net

type GuardianService(store: StorageDatabase.IStore) =

    member this.GetAllGuardians() : Result<seq<Guardian>, ServiceError> =
        Ok (
            InMemoryDatabase.all store.guardians
            |> Seq.choose (fun (id, name) ->
            match Guardian.build id name with
                | Ok guardian -> Some guardian
                | Error _ -> None)
        )

    member this.GetGuardian(name: string) : Result<Guardian, ServiceError> =
        match InMemoryDatabase.lookup name store.guardians with
        | None -> Error (ServiceError.NotFound "Guardian not found")
        | Some (id, name) ->
            match Guardian.build id name with
            | Ok guardian -> Ok guardian
            | Error _ -> Error (ServiceError.InvalidData "Invalid Guardian data")

    member this.AddGuardian(guardian: Guardian) : Result<Guardian, ServiceError> =
        match InMemoryDatabase.insert (Name.value guardian.Name) (GuardianId.value guardian.Id, Name.value guardian.Name ) store.guardians with
        | Ok () -> Ok (guardian)
        | Error (UniquenessError msg) -> Error (ServiceError.UniquenessError msg)

    member this.DecodeGuardian(json: string) : Result<Guardian, ServiceError> =
        match Decode.fromString Guardian.decode json with
        | Ok (Ok guardian) -> Ok guardian
        | Ok (Error err) -> 
            let errorMessage =
                match err with
                | Guardian.InvalidName _ -> "Invalid name"
                | Guardian.InvalidId _ -> "Invalid Guardian Id"
                | Guardian.MissingField msg -> "Missing field " + msg
            Error (ServiceError.InvalidData errorMessage)
        | Error err -> Error (ServiceError.InvalidData err)