module RouteUtils

open System
open System.Text.Json
open System.Text.Json.Serialization
open System.Threading.Tasks
open Falco
open Microsoft.AspNetCore.Http
open NotaFiscal.Domain.Rop
open NotaFiscal.Domain.DomainEvents

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
    

let getIdFromRoute (ctx: HttpContext) =
    let route = Request.getRoute ctx
    route.GetString "id" |> Guid.Parse

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
