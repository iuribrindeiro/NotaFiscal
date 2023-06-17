module NotaFiscal.Domain.Requirements.CriarNota

open System
open NotaFiscal.Domain.Dto.ServicoDto
open NotaFiscal.Domain.NotaFiscalStatus
open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.Dto.TomadorDto
open NotaFiscal.Domain.Rop


type CriarNotaDto =
    { Id: Guid; Tomador: TomadorDto; Servico: ServicoDto }
    
let criarNota dto =
    let criarNota' id tomador servico =
        { Id = id; Tomador = tomador; Servico = servico; Status = Pendente }
    
    criarNota'
        dto.Id
        <!> toTomadorDomain dto.Tomador
        <*> toServicoDomain dto.Servico
        