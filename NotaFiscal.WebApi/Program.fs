open System
open System.Threading.Tasks
open Falco
open Falco.Routing
open Falco.HostBuilder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open NotaFiscal.Data.Mongo

type ApiErrors =
    | NotaFiscalNaoEncontrada
    | FalhaComunicarBancoDados of FalhaComunicarBancoDados

let getRepository (ctx: HttpContext) =
    ctx.RequestServices.GetRequiredService<INotaFiscalRepository>()

let findNotaByIdAsync (ctx: HttpContext) id =
    let repository = getRepository ctx
    task {
        let! notaFiscalResult = repository.FindByIdAsync id
        
        return notaFiscalResult
               |> mapFailuresR FalhaComunicarBancoDados
               >>= failIfNoneR NotaFiscalNaoEncontrada
    }
    
let toApiResult result ctx =
    match result with
    | Success (e, _) -> Response.ofJson e ctx
    | Failure [NotaFiscalNaoEncontrada]
         -> (Response.withStatusCode 404 >> Response.ofPlainText "Not found") ctx
    | _ -> (Response.withStatusCode 500 >> Response.ofPlainText "Erro inesperado") ctx
    

let getIdFromRoute (ctx: HttpContext) =
    let route = Request.getRoute ctx
    route.GetString "id" |> Guid.Parse

let getHandler: HttpHandler = 
    task {
        let! notaFiscal = getIdFromRoute ctx |> findNotaByIdAsync ctx
        return notaFiscal |> toApiResult
    }

let configureServices (services: IServiceCollection) =
    let config = configuration [||] {
        required_json "appsettings.json"
    }
    
    services.Configure<MongoDbOptions>(config.GetSection("MongoDb")) |> ignore
    
    services
        .AddScoped<INotaFiscalRepository, NotaFiscalRepository>()
        

webHost [||] {
    add_service configureServices
    endpoints [ "/{id:guid}" (Request.mapRoute (getHandler)) ]
}