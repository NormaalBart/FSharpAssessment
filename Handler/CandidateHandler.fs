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

let getUpgradableCandidates: HttpHandler =
    fun next ctx ->
        task {
            let candidateService = ctx.GetService<CandidateService>()
            let sessionService = ctx.GetService<SessionService>()
            let result = 
                match candidateService.GetAllCandidates() with
                | Ok candidates ->
                    candidates 
                    |> Seq.filter (fun candidate -> 
                        match Diploma.nextDiploma candidate.Diploma with
                        | Diploma.A | Diploma.B | Diploma.C -> true
                        | Diploma.None -> false
                    )
                    |> Seq.filter (fun candidate ->
                        match sessionService.GetSessions (Name.value candidate.Name) with
                        | Ok sessions -> Candidate.canUpgradeToDiploma (Diploma.nextDiploma candidate.Diploma) sessions
                        | Error _ -> false
                    )
                    |> Ok
                | Error error -> Error error
            return! respondWithJsonSeq Candidate.encode result next ctx
        }


let handlers: HttpHandler = 
    choose [
          POST >=> route "/candidate" >=> addCandidate
          GET >=> route "/candidate" >=> getCandidates
          GET >=> route "/candidate/upgradable" >=> getUpgradableCandidates
          GET >=> routef "/candidate/%s" getCandidate
    ]