module Database.SeedStore

open System
open Database.InstoreDatabase
open Application

type Store() =
       interface IStore with

        member this.candidates: InMemoryDatabase<string,(string * DateTime * string * string)> =
            MockData.candidates

        member this.guardians: InMemoryDatabase<string,(string * string)> = 
            MockData.guardians

        member this.sessions: InMemoryDatabase<(string * DateTime),(string * string * DateTime * int)> = 
            MockData.sessions