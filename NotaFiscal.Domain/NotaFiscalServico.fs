module NotaFiscal.Domain.NotaFiscalServico

open Tomador
open NotaFiscal.Domain.Servico
open NotaFiscalStatus
open System

type NotaFiscalServico =
    { Id: Guid
      Tomador: Tomador
      Servico: Servico
      Status: NotaFiscalServicoStatus }