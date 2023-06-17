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
open NotaFiscal.Data
open NotaFiscal.Data.NotaFiscalRepository
open NotaFiscal.Domain
open NotaFiscal.Domain.Dto.Errors
open NotaFiscal.Domain.Dto.NotaFiscalServicoDto
open NotaFiscal.Domain.Requirements.CriarNota

type ApiErrors =
    | NotFound of string
    | UnprocessableEntity of NotaFiscalMapDomainErrors
    | DeserializationFail
    | InternalServerError

let getNotaFiscalCollection (ctx: HttpContext) =
    ctx.RequestServices.GetRequiredService<IMongoCollection<NotaFiscalServicoDto>>()


let findNotaByIdAsync (ctx: HttpContext) id =
    let mongoCollection = getNotaFiscalCollection ctx
    task {
        let! notaFiscalResult = findNotaNotaById mongoCollection id
        
        let mapErrors (error: DatabaseError) =
            match error with
            | DatabaseError.NotFound(entity, id) -> NotFound $"{entity} com id {id} não encontrada"
            | DatabaseError.DatabaseException _ -> InternalServerError
            | FailedToDeserialize _ -> InternalServerError
        
        return notaFiscalResult
               |> mapFailuresR mapErrors
    }

let isUnprocessableEntity err =
    match err with
    | UnprocessableEntity _ -> true
    | _ -> false

let toApiResult result ctx =
    match result with
    | Success (e, _) -> Response.ofJson e ctx
    | Failure [NotFound message]
         -> (Response.withStatusCode 404 >> Response.ofPlainText message) ctx
    | Failure f when List.exists isUnprocessableEntity f 
         -> (Response.withStatusCode 422 >> Response.ofPlainText "Unprocessable entity") ctx
    | _ -> (Response.withStatusCode 500 >> Response.ofPlainText "Erro inesperado") ctx
    
let toApiResultAsync ctx taskResult: Task =
    task {
        let! result = taskResult
        return! toApiResult result ctx
    }
    

let deserializeRequest handler =
    let options = JsonSerializerOptions()
    options.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
    
    Request.mapJsonOption options handler

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
        
let getHandler (ctx: HttpContext): Task = 
    task {
        let! notaFiscalResult = getIdFromRoute ctx |> findNotaByIdAsync ctx
        return! toApiResult notaFiscalResult ctx
    }


let postHandler (request: CriarNotaDto) (ctx: HttpContext) =
    let collection = getNotaFiscalCollection ctx
    let salvar nota = (salvarNota collection nota) |> mapFailuresRAsync (fun _ -> InternalServerError)
    
    criarNota request
        |> mapFailuresR UnprocessableEntity
    >>= salvar
    |> toApiResultAsync ctx
    
    
    
webHost [||] {
    add_service configureServices
    endpoints [
        get "/{id:guid}" getHandler
        post "/" <| deserializeRequest postHandler 
    ]
}