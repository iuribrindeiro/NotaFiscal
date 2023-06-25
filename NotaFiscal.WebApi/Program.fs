open System
open System.Text.Json
open System.Text.Json.Serialization
open System.Threading.Tasks
open Falco
open Falco.Routing
open Falco.HostBuilder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver
open NotaFiscal.Data.NotaFiscalRepository
open NotaFiscal.Domain
open NotaFiscal.Domain.ApplicationErrors
open NotaFiscal.Domain.Dto.NotaFiscalServicoDto
open NotaFiscal.Domain.Requirements.CriarNota


let isUnprocessableEntity err =
    match err with
    | ValidationError _ -> true
    | _ -> false

let toApiResult ctx result =
    match result with
    | Success (e, _) -> Response.ofJson e ctx
    | Failure [NotFound (resource, id)]
         -> (Response.withStatusCode 404 >> Response.ofPlainText $"Could not find {resource} by id {id}") ctx
    | Failure f when List.exists isUnprocessableEntity f 
         -> (Response.withStatusCode 422 >> Response.ofPlainText "Unprocessable entity") ctx
    | _ -> (Response.withStatusCode 500 >> Response.ofPlainText "Erro inesperado") ctx
    
let toApiResultAsync ctx taskResult: Task =
    task {
        let! result = taskResult
        return! toApiResult ctx result
    }

let getNotaFiscalCollection (ctx: HttpContext) =
    ctx.RequestServices.GetRequiredService<IMongoCollection<NotaFiscalServicoDto>>()

let findNotaByIdAsync (ctx: HttpContext) (id: Guid) =
    let mongoCollection = getNotaFiscalCollection ctx
    findNotaNotaById mongoCollection id
    
let salvarNotaFiscalAsync ctx nota =
    let mongoCollection = getNotaFiscalCollection ctx
    salvarNota mongoCollection nota
    

let deserializeRequest handler =
    let options = JsonSerializerOptions()
    options.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
    options.PropertyNameCaseInsensitive <- true
    
    let tryDeserialize (handler: 'T -> 'a -> Task) (json: string) =
        try
            let req = JsonSerializer.Deserialize<'T>(json, options)
            handler req
        with _ -> Response.withStatusCode 422 >> Response.ofPlainText $"Invalid request body."
            
    
    Request.bodyString (tryDeserialize handler)

let getIdFromRoute (ctx: HttpContext) =
    let route = Request.getRoute ctx
    route.GetString "id" |> Guid.Parse

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
        
let getHandler (ctx: HttpContext) = 
    getIdFromRoute ctx
        |> findNotaByIdAsync ctx
        |> toApiResultAsync ctx


let postHandler (req: CriarNotaDto) (ctx: HttpContext) =
    criarNota req
        >>= salvarNotaFiscalAsync ctx
    |> toApiResultAsync ctx
    
    
    
webHost [||] {
    add_service configureServices
    endpoints [
        get "/{id:guid}" getHandler
        post "/" <| deserializeRequest postHandler 
    ]
}