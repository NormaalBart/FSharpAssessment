namespace Models

open Thoth.Json.Net
open Models

type Guardian =
    { Id: GuardianId
      Name: Name}

module Guardian =

    type Error =
        | InvalidId of GuardianId.Error
        | InvalidName of Name.Error
        | MissingField of string

    let build (id: string) (name: string) : Result<Guardian, Error> =
        match GuardianId.create id, Name.create name with
        | Ok i, Ok n -> Ok { Id = i; Name = n }
        | Error e, _ -> Error (InvalidId e)
        | _, Error e -> Error (InvalidName e)

    let encode (guardian: Guardian) : JsonValue =
        Encode.object
            [ "id", Encode.string (GuardianId.value guardian.Id)
              "name", Encode.string (Name.value guardian.Name) ]

    let decode : Decoder<Result<Guardian, Error>> =
        Decode.object (fun get ->
            let id = get.Optional.Field "id" Decode.string
            let name = get.Optional.Field "name" Decode.string
            match id, name with
            | Some i, Some n -> build i n
            | None, _ -> Error (MissingField "id")
            | _, None -> Error (MissingField "name"))
