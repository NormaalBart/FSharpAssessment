module IStore

open Database.InstoreDatabase
open System
open Models

type IStore = 

    abstract candidates: InMemoryDatabase<string, string * DateTime * string * string>

    abstract guardians: InMemoryDatabase<string, string * string>

    abstract sessions: InMemoryDatabase<string * DateTime, string * PoolType * DateTime * int>
