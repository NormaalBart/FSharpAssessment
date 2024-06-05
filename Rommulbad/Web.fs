module Rommulbad.Web

open Models
open Services
open Giraffe
open Thoth.Json.Net
open Thoth.Json.Giraffe
open Microsoft.AspNetCore.Http

module ErrorHandling =
    let handleServiceError (error: ServiceError) (next: HttpFunc) (ctx: HttpContext) =
        match error with
        | ServiceError.InvalidData msg -> RequestErrors.BAD_REQUEST msg next ctx
        | ServiceError.NotFound msg -> RequestErrors.NOT_FOUND msg next ctx
        | ServiceError.UniquenessError msg -> RequestErrors.CONFLICT msg next ctx

let getCandidates: HttpHandler =
    fun next ctx ->
        task {
            let candidateService = ctx.GetService<CandidateService>()
            match candidateService.GetAllCandidates() with
            | Ok candidates -> return! ThothSerializer.RespondJsonSeq candidates Candidate.encode next ctx
            | Error error -> return! ErrorHandling.handleServiceError error next ctx
        }

let getCandidate (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let candidateService = ctx.GetService<CandidateService>()
            match candidateService.GetCandidate(name) with
            | Ok candidate -> return! ThothSerializer.RespondJson candidate Candidate.encode next ctx
            | Error error -> return! ErrorHandling.handleServiceError error next ctx
        }

let addSession (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let! body = ctx.ReadBodyFromRequestAsync()
            let sessionService = ctx.GetService<SessionService>()
            match sessionService.DecodeSession(body) with
            | Ok session ->
                match sessionService.AddSession(name, session) with
                | Ok () -> return! text "OK" next ctx
                | Error error -> return! ErrorHandling.handleServiceError error next ctx
            | Error error -> return! ErrorHandling.handleServiceError error next ctx
        }

let getSessions (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let sessionService = ctx.GetService<SessionService>()
            match sessionService.GetSessions(name) with
            | Ok sessions -> return! ThothSerializer.RespondJsonSeq sessions Session.encode next ctx
            | Error error -> return! ErrorHandling.handleServiceError error next ctx
        }

let getTotalMinutes (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let sessionService = ctx.GetService<SessionService>()
            match sessionService.GetTotalMinutes(name) with
            | Ok total -> return! ThothSerializer.RespondJson total Encode.int next ctx
            | Error error -> return! ErrorHandling.handleServiceError error next ctx
        }

let getEligibleSessions (name: string, diploma: string) : HttpHandler =
    fun next ctx ->
        task {
            let sessionService = ctx.GetService<SessionService>()
            match sessionService.GetEligibleSessions(name, diploma) with
            | Ok sessions -> return! ThothSerializer.RespondJsonSeq sessions Session.encode next ctx            
            | Error error -> return! ErrorHandling.handleServiceError error next ctx
        }

let getTotalEligibleMinutes (name: string, diploma: string) : HttpHandler =
    fun next ctx ->
        task {
            let sessionService = ctx.GetService<SessionService>()
            match sessionService.GetTotalEligibleMinutes(name, diploma) with
            | Ok total -> return! ThothSerializer.RespondJson total Encode.int next ctx
            | Error error -> return! ErrorHandling.handleServiceError error next ctx
        }

let routes: HttpHandler =
    choose
        [ GET >=> route "/candidate" >=> getCandidates
          GET >=> routef "/candidate/%s" getCandidate
          POST >=> routef "/candidate/%s/session" addSession
          GET >=> routef "/candidate/%s/session" getSessions
          GET >=> routef "/candidate/%s/session/total" getTotalMinutes
          GET >=> routef "/candidate/%s/session/%s" getEligibleSessions
          GET >=> routef "/candidate/%s/session/%s/total" getTotalEligibleMinutes 
        ]
