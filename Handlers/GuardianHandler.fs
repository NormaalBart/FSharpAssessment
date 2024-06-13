module GuardianHandler

open JsonParser
open Models
open Services
open Giraffe

let getGuardians: HttpHandler =
    fun next ctx ->
        task {
            let guardianService = ctx.GetService<GuardianService>()
            let result = guardianService.GetAllGuardians()
            return! respondWithJsonSeq Guardian.encode result next ctx
        }

let getGuardian (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let guardianService = ctx.GetService<GuardianService>()
            let result = guardianService.GetGuardian(name)
            return! respondWithJsonSingle Guardian.encode result next ctx
        }

let addGuardian: HttpHandler =
    fun next ctx ->
        task {
            let! body = ctx.ReadBodyFromRequestAsync()
            let guardianService = ctx.GetService<GuardianService>()
            let result =
                match guardianService.DecodeGuardian(body) with
                | Ok guardian -> guardianService.AddGuardian guardian
                | Error error -> Error error
            return! respondWithJsonSingle Guardian.encode result next ctx
        }

let requestHandlers: HttpHandler = 
    choose [
          GET >=> route "/guardian" >=> getGuardians
          POST >=> route "/guardian" >=> addGuardian
          GET >=> routef "/guardian/%s" getGuardian
    ]