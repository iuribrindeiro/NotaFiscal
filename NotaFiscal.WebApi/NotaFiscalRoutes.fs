module NotaFiscalRoutes

open Microsoft.AspNetCore.Http
open RouteUtils
open NotaFiscalRoutesUtils
open NotaFiscal.Domain.Requirements.CriarNota

let getNotaHandler (ctx: HttpContext) = 
    getIdFromRoute ctx
        |> findNotaByIdDbAsync ctx
        |> toNotaFiscalDtoAsync
        |> toApiResultAsync ctx


let postNotaHandler (req: CriarNotaDto) (ctx: HttpContext) =
    criarNota req
        |> saveChangesDbAsync ctx
        |> toNotaFiscalDtoAsync
        |> toApiResultAsync ctx