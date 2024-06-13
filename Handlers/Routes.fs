module Routes

open Giraffe

let routes: HttpHandler =
    choose
        [ 
          SessionHandler.requestHandlers
          GuardianHandler.requestHandlers
          CandidateHandler.requestHandlers
        ]
