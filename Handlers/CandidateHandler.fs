module CandidateHandler

open JsonParser
open Models
open Services
open Giraffe

let getCandidates: HttpHandler =
    fun next ctx ->
        task {
            let candidateService = ctx.GetService<CandidateService>()
            let result = candidateService.GetAllCandidates()
            return! respondWithJsonSeq Candidate.encode result next ctx
        }

let getCandidate (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let candidateService = ctx.GetService<CandidateService>()
            let result = candidateService.GetCandidate(name)
            return! respondWithJsonSingle Candidate.encode result next ctx
        }

let addCandidate: HttpHandler =
    fun next ctx ->
        task {
            let! body = ctx.ReadBodyFromRequestAsync()
            let candidateService = ctx.GetService<CandidateService>()
            let result =
                match candidateService.DecodeCandidate(body) with
                | Ok candidate -> candidateService.AddCandidate candidate
                | Error error -> Error error
            return! respondWithJsonSingle Candidate.encode result next ctx
        }

let requestHandlers: HttpHandler = 
    choose [
          GET >=> route "/candidate" >=> getCandidates
          POST >=> route "/candidate" >=> addCandidate
          GET >=> routef "/candidate/%s" getCandidate
    ]
