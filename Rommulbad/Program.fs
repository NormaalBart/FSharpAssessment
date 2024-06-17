open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open StorageDatabase
open Database.SeedStore
open Services
open Thoth.Json.Giraffe
open Thoth.Json.Net
open Database.SeedStore

let configureApp (app: IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe Routes.routes

let configureServices (services: IServiceCollection) =
    // Add Giraffe dependencies
    services
        .AddGiraffe()
        .AddSingleton<IStore>(Database.SeedStore.Store())
        .AddSingleton<SessionService>(fun serviceProvider -> 
            let store = serviceProvider.GetService<IStore>()
            SessionService(store)
        )
        .AddSingleton<CandidateService>(fun serviceProvider -> 
            let store = serviceProvider.GetService<IStore>()
            CandidateService(store)
        )
        .AddSingleton<GuardianService>(fun serviceProvider -> 
            let store = serviceProvider.GetService<IStore>()
            GuardianService(store)
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
