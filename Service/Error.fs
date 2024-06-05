namespace Services

type ServiceError =
    | NotFound of string
    | InvalidData of string
    | UniquenessError of string