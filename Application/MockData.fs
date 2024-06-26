﻿module MockData

open Database.InstoreDatabase
open Models
open System

let guardians = 
            [ ("123-ABCD", "Jan Janssen")
              ("234-FDEG", "Marie Moor")
              ("999-ZZZZ", "Margeet van Lankerveld") ]
            |> Seq.choose (fun (id, name) ->
                match Guardian.build id name with
                | Ok guardian -> Some (GuardianId.value guardian.Id, (GuardianId.value guardian.Id, Name.value guardian.Name))
                | _ -> None)
            |> InMemoryDatabase.ofSeq
            
let candidates = 
            [ ("Eleanor", DateTime(2016, 1, 9), "123-ABCD", "A")
              ("Camiel", DateTime(2015, 11, 3), "123-ABCD", "C")
              ("Lore", DateTime(2018, 8, 30), "999-ZZZZ", "") ]
            |> Seq.choose (fun (n, bd, gi, dpl) ->
                match Candidate.build n bd gi dpl with
                | Ok candidate -> Some (Name.value candidate.Name, (Name.value candidate.Name, bd, GuardianId.value candidate.GuardianId, Diploma.value candidate.Diploma))
                | Error _ -> None)
            |> InMemoryDatabase.ofSeq

let sessions = 
            [ ("Eleanor", "Shallow", DateTime(2024, 2, 2), 3)
              ("Eleanor", "Shallow", DateTime(2024, 3, 2), 5)
              ("Eleanor", "Shallow", DateTime(2024, 3, 2), 10)
              ("Eleanor", "Deep", DateTime(2024, 4, 1), 30)
              ("Eleanor", "Deep", DateTime(2024, 5, 2), 10)
              ("Eleanor", "Deep", DateTime(2024, 5, 3), 15)
              ("Camiel", "Shallow", DateTime(2023, 4, 10), 15)
              ("Camiel", "Deep", DateTime(2023, 4, 17), 10)
              ("Camiel", "Deep", DateTime(2023, 5, 24), 20)
              ("Camiel", "Deep", DateTime(2023, 5, 14), 10)
              ("Camiel", "Deep", DateTime(2023, 6, 13), 20)
              ("Camiel", "Deep", DateTime(2023, 6, 17), 10)
              ("Camiel", "Deep", DateTime(2023, 7, 10), 20)
              ("Camiel", "Deep", DateTime(2023, 7, 17), 10)
              ("Camiel", "Deep", DateTime(2023, 8, 10), 20)
              ("Camiel", "Deep", DateTime(2023, 8, 17), 10)
              ("Camiel", "Deep", DateTime(2023, 9, 10), 20)
              ("Camiel", "Deep", DateTime(2023, 9, 17), 10)
              ("Camiel", "Deep", DateTime(2023, 10, 10), 20)
              ("Camiel", "Deep", DateTime(2023, 10, 17), 10)
              ("Camiel", "Deep", DateTime(2023, 11, 10), 20)
              ("Camiel", "Deep", DateTime(2023, 11, 17), 10)
              ("Camiel", "Deep", DateTime(2023, 12, 10), 20)
              ("Camiel", "Deep", DateTime(2023, 12, 17), 10)
              ("Lore", "Shallow", DateTime(2024, 6, 3), 1)
              ("Lore", "Shallow", DateTime(2024, 6, 10), 30)
              ("Lore", "Shallow", DateTime(2024, 6, 11), 30)
              ("Lore", "Shallow", DateTime(2024, 6, 12), 30)
              ("Lore", "Shallow", DateTime(2024, 6, 13), 30) ]
            |> Seq.choose (fun (n, pool, date, min) ->
                match Session.build pool date min with
                | Ok session -> Some ((n, date), (n, PoolType.value session.Pool, session.Date, Minutes.value session.Minutes))
                | Error _ -> None)
            |> InMemoryDatabase.ofSeq