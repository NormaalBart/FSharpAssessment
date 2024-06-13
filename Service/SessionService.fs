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

    member this.GetTotalMinutes(name: string) : Result<int, ServiceError> =
        Ok (
            getSessionsForUser name
            |> Seq.map (fun session -> Minutes.value session.Minutes)
            |> Seq.sum
        )

    member this.GetEligibleSessions(name: string, diploma: string) : Result<seq<Session>, ServiceError> =
        let shallowOk = diploma = "A"
        let minMinutes =
            match diploma with
            | "A" -> 1
            | "B" -> 10
            | _ -> 15

        let filter session =
            (session.Pool = Shallow || shallowOk) && (Minutes.value session.Minutes >= minMinutes)

        Ok (
            getSessionsForUser name
            |> Seq.filter filter
        )

    member this.GetTotalEligibleMinutes(name: string, diploma: string) : Result<int, ServiceError> =
        let shallowOk = diploma = "A"
        let minMinutes =
            match diploma with
            | "A" -> 1
            | "B" -> 10
            | _ -> 15

        let filter session =
            (session.Pool = Shallow || shallowOk) && (Minutes.value session.Minutes >= minMinutes)

        Ok (
            getSessionsForUser name
            |> Seq.filter filter
            |> Seq.map (fun session -> Minutes.value session.Minutes)
            |> Seq.sum
        )

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

