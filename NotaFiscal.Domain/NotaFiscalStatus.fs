module NotaFiscal.Domain.NotaFiscalStatus

open System

type NotaFiscalServicoStatus =
    | Pendente
    | AguardandoAutorizacao of AguardandoAutorizacaoStatus
    | Autorizada of AutorizadaStatus
    | ErroAutorizacao of ErroAutorizacaoStatus
    | Cancelada of CanceladaStatus

and AguardandoAutorizacaoStatus = NumeroLote * NumeroProtocolo * DataEmissao * Rps
and AutorizadaStatus = DataEmissao * Rps * NumeroNota
and ErroAutorizacaoStatus = DataEmissao * Rps * ErroComunicacao list
and CanceladaStatus = DataEmissao * Rps * NumeroNota * CodigoCancelamento

and ErroComunicacao =
    { Id: Guid
      CodigoMensagemAlerta: CodigoErro
      Mensagem: string
      Correcao: string option }

and Rps = { Numero: int; Serie: string; Tipo: TipoRps }

and TipoRps =
    | Rps
    | NotaFiscalConjugadaMista
    | Cupom

and CodigoCancelamento = string
and DataEmissao = DateTime
and NumeroNota = int
and NumeroProtocolo = string
and NumeroLote = string
and CodigoErro = string
and Correcao = string

let createRps numero serie tipo =
    { Numero = numero; Serie = serie; Tipo = tipo }

let createStatusAguardandoAutorizacao numeroLote numeroProtocolo dataEmissao rps =
    (numeroLote, numeroProtocolo, dataEmissao, rps)
    |> AguardandoAutorizacao

let createStatusAutorizada dataEmissao notaSolicitadaEnvioData numeroNota =
    (dataEmissao, notaSolicitadaEnvioData, numeroNota)
    |> Autorizada

let createStatusErroAutorizacao dataEmissao rps errosComunicacoes =
    (dataEmissao, rps, errosComunicacoes) |> ErroAutorizacao

let createStatusCancelada dataEmissao rps numeroNota codigoCancelamento =
    (dataEmissao, rps, numeroNota, codigoCancelamento)
    |> Cancelada