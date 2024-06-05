module Routes

open CandidateHandler
open SessionHandler
open GuardianHandler
open Giraffe

let routes: HttpHandler =
    choose
        [ GET >=> route "/candidate" >=> getCandidates
          POST >=> route "/candidate" >=> addCandidate
          GET >=> routef "/candidate/%s" getCandidate
          POST >=> routef "/candidate/%s/session" addSession
          GET >=> routef "/candidate/%s/session" getSessions
          GET >=> routef "/candidate/%s/session/total" getTotalMinutes
          GET >=> routef "/candidate/%s/session/%s" getEligibleSessions
          GET >=> routef "/candidate/%s/session/%s/total" getTotalEligibleMinutes
          GET >=> route "/guardian" >=> getGuardians
          POST >=> route "/guardian" >=> addGuardian
          GET >=> routef "/guardian/%s" getGuardian
        ]
