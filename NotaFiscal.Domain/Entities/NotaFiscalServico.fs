module NotaFiscal.Domain.NotaFiscalServico

open System
open NotaFiscal.Domain.Entities
open NotaFiscal.Domain.Entities.Servico
open NotaFiscal.Domain.Entities.NotaFiscalStatus

type NotaFiscalServico =
    { Id: Guid
      Tomador: Tomador
      Servico: Servico
      Status: NotaFiscalServicoStatus }
