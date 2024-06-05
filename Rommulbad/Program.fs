﻿open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Thoth.Json.Giraffe
open Thoth.Json.Net
open Rommulbad.Web
open Database.SeedStore
open Services

let configureApp (app: IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe routes

let configureServices (services: IServiceCollection) =
    // Add Giraffe dependencies
    services
        .AddGiraffe()
        .AddSingleton<Store>(Store())
        .AddSingleton<SessionService>(fun serviceProvider -> 
            let store = serviceProvider.GetService<Store>()
            SessionService(store)
        )
        .AddSingleton<CandidateService>(fun serviceProvider -> 
            let store = serviceProvider.GetService<Store>()
            CandidateService(store)
        )
        .AddSingleton<Json.ISerializer>(ThothSerializer(skipNullField = false, caseStrategy = CaseStrategy.CamelCase))
    |> ignore

[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder.Configure(configureApp).ConfigureServices(configureServices)
            |> ignore)
        .Build()
        .Run()

    0
