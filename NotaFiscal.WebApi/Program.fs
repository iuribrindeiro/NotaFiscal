open Falco
open Falco.Routing
open Falco.HostBuilder
open NotaFiscal.Domain.ApplicationErrors
open NotaFiscal.WebApplication.NotaFiscalRequests

type ApiErrors =
    | FailedDeserializeJson
    | DomainError of NotaFiscalErrors

// let toApiResult (result: OperationResult<'a, ApiErrors>) =
//     match result with
//     | Success(x, _) -> Successful.OK x
//     | Failure errors ->
//         match errors with
//         | [ FailedDeserializeJson ] ->
//             RequestErrors.BAD_REQUEST "O request não pode ser deserializado"
//         | _ -> RequestErrors.BAD_REQUEST errors

let salvarNota r =
    salvarNotaFiscal r
    |> mapFailuresR DomainError

// let tryBindJson
//     (ctx: HttpContext)
//     : Task<OperationResult<CriarNotaFiscalRequest, ApiErrors>>
//     =
//     task {
//         try
//             let! model = ctx.BindJsonAsync<CriarNotaFiscalRequest>()
//             return (model |> succeed)
//         with ex ->
//             return (fail FailedDeserializeJson)
//     }

let postHandler: HttpHandler =
    fun ctx ->
        task {
            let requestFromJson json =
                FSharp.Json.Json.deserialize<CriarNotaFiscalRequest> json

            let! json = Request.getBodyString ctx


            let result (nota: CriarNotaFiscalRequest) =
                if Option.isSome nota.Tomador then "some!" else "none!"


            let request = requestFromJson json |> result

            return Response.ofPlainText request ctx
        }


webHost [||] { endpoints [ post "/" postHandler ] }