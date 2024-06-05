namespace Models

type GuardianId = private GuardianId of string

module GuardianId =

    type Error =
        | InvalidGuardianId of string

    let create (guardianId: string) : Result<GuardianId, Error> =
        if System.Text.RegularExpressions.Regex.IsMatch(guardianId, @"^\d{3}-[A-Z]{4}$") then
            Ok (GuardianId guardianId)
        else
            Error (InvalidGuardianId "GuardianId format is invalid")

    let value (GuardianId guardianId) = guardianId
