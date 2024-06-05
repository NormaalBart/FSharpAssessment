namespace Models

open System

type Name = private Name of string

module Name =

    type Error =
        | InvalidName of string

    let create (name: string) : Result<Name, Error> =
        if name |> String.forall (fun c -> Char.IsLetter(c) || c = ' ') && name.Trim().Length > 0 then
            Ok (Name name)
        else
            Error (InvalidName "Name contains invalid characters")

    let value (Name name) = name
