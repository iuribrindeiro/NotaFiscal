module NotaFiscalRoutesUtils

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open MongoDB.Driver
open NotaFiscal.Domain.Rop
open NotaFiscal.Data.NotaFiscalRepository
open Microsoft.Extensions.DependencyInjection
open NotaFiscal.Domain.Dto.NotaFiscalServicoDto

let getNotaFiscalCollection (ctx: HttpContext) =
    ctx.RequestServices.GetRequiredService<IMongoCollection<NotaFiscalServicoDto>>()

let findNotaByIdDbAsync (ctx: HttpContext) (id: Guid) =
    let mongoCollection = getNotaFiscalCollection ctx
    findNotaNotaById mongoCollection id

let saveChangesDbAsync ctx notaFiscalR =
    let mongoCollection = getNotaFiscalCollection ctx
    salvarNota mongoCollection |> checkRAsync notaFiscalR

let toNotaFiscalDtoAsync notaRAsync =
    mapRAsync notaRAsync (fromDomain >> Task.FromResult)