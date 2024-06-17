module Application

open Database.InstoreDatabase
open System

type IStore = 

    abstract candidates: InMemoryDatabase<string, string * DateTime * string * string>

    abstract guardians: InMemoryDatabase<string, string * string>

    abstract sessions: InMemoryDatabase<string * DateTime, string * string * DateTime * int>
    