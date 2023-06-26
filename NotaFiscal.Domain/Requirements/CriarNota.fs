module NotaFiscal.Domain.Requirements.CriarNota

open System
open NotaFiscal.Domain.DomainEvents
open NotaFiscal.Domain.Dto.ServicoDto
open NotaFiscal.Domain.NotaFiscalStatus
open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.Dto.TomadorDto
open NotaFiscal.Domain.Rop


type CriarNotaDto =
    { Id: Guid; Tomador: TomadorDto; Servico: ServicoDto }
    
let criarNota dto =
    let notaId =
        if dto.Id = Guid.Empty then Guid.NewGuid() else dto.Id
    
    let criarNota' tomador servico =
        { Id = notaId; Tomador = tomador; Servico = servico; Status = Pendente }
    
    criarNota'
        <!> toTomadorDomain dto.Tomador
        <*> toServicoDomain dto.Servico
    |> mapFailuresR ValidationError
    |> mapMsgR NotaFiscalCriada
        