open System
open Falco.Routing
open Falco.HostBuilder
open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver
open RouteUtils
open NotaFiscalRoutes
open NotaFiscal.Domain.Dto.NotaFiscalServicoDto

let configureServices (services: IServiceCollection) =
    let config = configuration [||] {
        required_json "appsettings.json"
    }
    
    let connectionString = config.GetSection("MongoDb:ConnectionString").Value
    let databaseName = config.GetSection("MongoDb:DatabaseName").Value
    
    let mongoClientFactory =
        MongoClient(connectionString)
    let notaFiscalCollectionFactory (sp: IServiceProvider) =
        sp.GetRequiredService<MongoClient>()
            .GetDatabase(databaseName)
            .GetCollection<NotaFiscalServicoDto>(nameof(NotaFiscalServicoDto))
    
    services
        .AddSingleton<MongoClient>(mongoClientFactory)
        .AddScoped<IMongoCollection<NotaFiscalServicoDto>>(notaFiscalCollectionFactory)

webHost [||] {
    add_service configureServices
    endpoints [
        get "/{id:guid}" getNotaHandler
        post "/" <| deserializeRequest postNotaHandler 
    ]
}