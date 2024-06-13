namespace Services

open Models
open Database.InstoreDatabase
open Database.SeedStore
open Thoth.Json.Net

type SessionService(store: Store) =

    let getSessionsForUser (name: string) : seq<Session> =
        InMemoryDatabase.filter (fun (n, pool, date, minutes) -> n = name) store.sessions
        |> Seq.choose (fun (_, pool, date, minutes) ->
            match Session.build pool date minutes with
            | Ok session -> Some session
            | Error _ -> None
        )

    member this.AddSession(name: string, session: Session) : Result<Session, ServiceError> =
        match InMemoryDatabase.insert (name, session.Date) (name, session.Pool, session.Date, Minutes.value session.Minutes) store.sessions with
        | Ok () -> Ok session
        | Error (UniquenessError msg) -> Error (ServiceError.UniquenessError msg)

    member this.GetSessions(name: string) : Result<seq<Session>, ServiceError> =
        Ok (getSessionsForUser name)

    member this.GetEligibleSessions(name: string, diploma: string) : Result<seq<Session>, ServiceError> =
        match Diploma.create diploma with
            | Ok diploma -> Ok (
                getSessionsForUser name
                |> Seq.filter (fun session -> Session.isApplicableForDiploma diploma session)
                )
            | Error error -> Error(InvalidData "invalid diploma")

    member this.GetTotalEligibleMinutes(name: string, diploma: string) : Result<int, ServiceError> =
        match this.GetEligibleSessions(name, diploma) with
            | Ok session -> Ok (
                session|> Seq.map (fun session -> Minutes.value session.Minutes)
                |> Seq.sum
                )
            | Error error -> Error error

    member this.DecodeSession(json: string) : Result<Session, ServiceError> =
        match Decode.fromString Session.decode json with
        | Ok (Ok session) -> Ok session
        | Ok (Error err) -> 
            let errorMessage =
                match err with
                | Session.InvalidMinutes _ -> "Invalid minutes"
                | Session.InvalidPoolType _ -> "Invalid pool type"
                | Session.MissingField msg -> "Missing field " + msg
            Error (ServiceError.InvalidData errorMessage)
        | Error err -> Error (ServiceError.InvalidData err)

