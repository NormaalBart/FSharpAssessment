namespace Database.SeedStore

open System
open Database.InstoreDatabase
open Models

/// Here a store is created that contains the following tables with the following attributes
///
/// candidates (primary key is name)
/// - name (consists of words separated by spaces)
/// - date of birth
/// - guardian id (see guardian id)
/// - highest swimming diploma (A, B, or C, with C being the highest)
///
/// sessions (primary key is compound: candidate name and date)
/// - candidate name (foreign key to candidates)
/// - date
/// - minutes (int)
///
/// guardians
/// - id (3 digits followed by dash and 4 letters, e.g. 133-LEET)
/// - name (consists of words separated by spaces)

type Store() =
    member val candidates: InMemoryDatabase<Name, Name * DateTime * GuardianId * Diploma> =
        [ ("Eleanor", DateTime(2016, 1, 9), "123-ABCD", "A")
          ("Camiel", DateTime(2015, 11, 3), "123-ABCD", "C")
          ("Lore", DateTime(2018, 8, 30), "999-ZZZZ", "") ]
        |> Seq.choose (fun (n, bd, gi, dpl) ->
            match Candidate.build n bd gi dpl with
            | Ok candidate -> Some (candidate.Name, (candidate.Name, bd, candidate.GuardianId, candidate.Diploma))
            | Error _ -> None)
        |> InMemoryDatabase.ofSeq

    member val sessions: InMemoryDatabase<string * DateTime, string * PoolType * DateTime * Minutes> =
        [ ("Eleanor", Shallow, DateTime(2024, 2, 2), 3)
          ("Eleanor", Shallow, DateTime(2024, 3, 2), 5)
          ("Eleanor", Shallow, DateTime(2024, 3, 2), 10)
          ("Eleanor", Deep, DateTime(2024, 4, 1), 30)
          ("Eleanor", Deep, DateTime(2024, 5, 2), 10)
          ("Eleanor", Deep, DateTime(2024, 5, 3), 15)
          ("Camiel", Shallow, DateTime(2023, 4, 10), 15)
          ("Camiel", Deep, DateTime(2023, 4, 17), 10)
          ("Camiel", Deep, DateTime(2023, 5, 24), 20)
          ("Camiel", Deep, DateTime(2023, 5, 14), 10)
          ("Camiel", Deep, DateTime(2023, 6, 13), 20)
          ("Camiel", Deep, DateTime(2023, 6, 17), 10)
          ("Camiel", Deep, DateTime(2023, 7, 10), 20)
          ("Camiel", Deep, DateTime(2023, 7, 17), 10)
          ("Camiel", Deep, DateTime(2023, 8, 10), 20)
          ("Camiel", Deep, DateTime(2023, 8, 17), 10)
          ("Camiel", Deep, DateTime(2023, 9, 10), 20)
          ("Camiel", Deep, DateTime(2023, 9, 17), 10)
          ("Camiel", Deep, DateTime(2023, 10, 10), 20)
          ("Camiel", Deep, DateTime(2023, 10, 17), 10)
          ("Camiel", Deep, DateTime(2023, 11, 10), 20)
          ("Camiel", Deep, DateTime(2023, 11, 17), 10)
          ("Camiel", Deep, DateTime(2023, 12, 10), 20)
          ("Camiel", Deep, DateTime(2023, 12, 17), 10)
          ("Lore", Shallow, DateTime(2024, 6, 3), 1)
          ("Lore", Shallow, DateTime(2024, 6, 10), 5) ]
        |> Seq.choose (fun (name, pool, date, min) ->
            match Session.build pool date min with
            | Ok session -> Some ((name, date), (name, session.Pool, session.Date, session.Minutes))
            | Error _ -> None)
        |> InMemoryDatabase.ofSeq

    member val guardians: InMemoryDatabase<GuardianId, GuardianId * Name> =
        [ ("123-ABCD", "Jan Janssen")
          ("234-FDEG", "Marie Moor")
          ("999-ZZZZ", "Margeet van Lankerveld") ]
        |> Seq.choose (fun (id, name) ->
            match Guardian.build id name with
            | Ok guardian -> Some (guardian.Id, (guardian.Id, guardian.Name))
            | _ -> None)
        |> InMemoryDatabase.ofSeq
