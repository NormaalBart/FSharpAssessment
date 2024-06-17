namespace Models

module Diploma =

    type Diploma =
        | None
        | A
        | B
        | C

    type Error =
        | InvalidDiploma of string

    let create (diploma: string) : Result<Diploma, Error> =
        match Util.makeUpperCase diploma with
        | "" -> Ok None
        | "A" -> Ok A
        | "B" -> Ok B
        | "C" -> Ok C
        | _ -> Error (InvalidDiploma "Diploma must be one of '', 'A', 'B', or 'C'")

    let value (diploma: Diploma) =
        match diploma with
        | None -> ""
        | A -> "A"
        | B -> "B"
        | C -> "C"

    let nextDiploma (diploma: Diploma) =
        match diploma with
        | None -> A
        | A -> B
        | B -> C
        | C -> None

    let minimumMinutesRequired (diploma: Diploma) =
        match diploma with
        | Diploma.None -> 0
        | Diploma.A -> 120
        | Diploma.B -> 150
        | Diploma.C -> 180