module NotaFiscal.Domain.NotaFiscalServico

open Tomador
open NotaFiscal.Domain.Servico.Servico
open NotaFiscalStatus
open System

type NotaFiscalServico =
    { Id: Guid
      Tomador: Tomador
      Servico: Servico
      Status: NotaFiscalServicoStatus }

let createNotaFiscalServico id tomadorOrError servicoOrError =
    let createNotaFiscalServico' tomador servico =
        { Id = id; Tomador = tomador; Servico = servico; Status = Pendente }

    createNotaFiscalServico' <!> tomadorOrError
    <*> servicoOrError