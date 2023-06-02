module NotaFiscal.Domain.NotaFiscalStatus

open System

type NotaFiscalServicoStatus =
    | Pendente
    | SolicitandoEmissao of SolicitandoEmissaoStatus
    | AguardandoAutorizacao of AguardandoAutorizacaoStatus
    | Autorizada of AutorizadaStatus
    | ErroAutorizacao of ErroAutorizacaoStatus
    | SolicitandoCancelamento of SolicitandoCancelamentoStatus
    | Cancelada of CanceladaStatus

and AguardandoAutorizacaoStatus =
    NumeroLote * NumeroProtocolo * DataEmissao * Rps

and SolicitandoEmissaoStatus = NumeroLote * DataEmissao * Rps

and SolicitandoCancelamentoStatus =
    DataEmissao * Rps * NumeroNota * CodigoCancelamento

and AutorizadaStatus = DataEmissao * Rps * NumeroNota
and ErroAutorizacaoStatus = DataEmissao * Rps * ErroComunicacao list

and CanceladaStatus =
    DataEmissao * Rps * NumeroNota * CodigoCancelamento * DataCancelamento

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
and DataCancelamento = DateTime
and NumeroNota = string
and NumeroProtocolo = string
and NumeroLote = string
and CodigoErro = string
and Correcao = string

let createRps numero serie tipo =
    { Numero = numero; Serie = serie; Tipo = tipo }

let createStatusAguardandoAutorizacao
    numeroLote
    numeroProtocolo
    dataEmissao
    rps
    =
    (numeroLote, numeroProtocolo, dataEmissao, rps)
    |> AguardandoAutorizacao

let createStatusAutorizada dataEmissao notaSolicitadaEnvioData numeroNota =
    (dataEmissao, notaSolicitadaEnvioData, numeroNota)
    |> Autorizada

let createStatusErroAutorizacao dataEmissao rps errosComunicacoes =
    (dataEmissao, rps, errosComunicacoes) |> ErroAutorizacao

let createStatusCancelada
    dataEmissao
    rps
    numeroNota
    codigoCancelamento
    dataCancelamento
    =
    (dataEmissao, rps, numeroNota, codigoCancelamento, dataCancelamento)
    |> Cancelada