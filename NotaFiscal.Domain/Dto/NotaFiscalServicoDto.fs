module NotaFiscal.Domain.Dto.NotaFiscalServicoDto

open System
open NotaFiscal.Domain
open NotaFiscal.Domain.ApplicationErrors

type NotaFiscalServicoDto =
    { Id: Guid; Tomador: TomadorDto.TomadorDto; Servico: ServicoDto.ServicoDto; Status: StatusDto.StatusDto }
let fromDomain (notaFiscal: NotaFiscalServico.NotaFiscalServico) : NotaFiscalServicoDto =
    { Id = notaFiscal.Id
      Tomador = TomadorDto.fromTomadorDomain notaFiscal.Tomador
      Servico = ServicoDto.fromServicoDomain notaFiscal.Servico
      Status = StatusDto.fromStatusDomain notaFiscal.Status }
    
let toDomain (notaFiscalDto: NotaFiscalServicoDto) =
    let createNotaFiscalServico' (tomador: Tomador.Tomador) (servico: Servico.Servico) : NotaFiscalServico.NotaFiscalServico =
        { Id = notaFiscalDto.Id
          Tomador = tomador
          Servico = servico
          Status = StatusDto.toStatusDomain notaFiscalDto.Status }
        
        
    createNotaFiscalServico'
      <!> TomadorDto.toTomadorDomain notaFiscalDto.Tomador
      <*> ServicoDto.toServicoDomain notaFiscalDto.Servico