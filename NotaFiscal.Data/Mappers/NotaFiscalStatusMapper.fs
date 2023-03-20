module NotaFiscal.Data.Mappers.NotaFiscalStatusMapper

open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.ValidationErrors
open NotaFiscal.Data.DbAutoGen
open System




let createDataEmissaoFromDb (dataEmissao) =
    match dataEmissao with
    | Some dataEmissao -> succeed dataEmissao
    | None -> fail DataEmissaoVazia

let createNumeroNotaFromDb (numeroNota: int option) =
    match numeroNota with
    | Some v when v > 0 -> succeed v
    | _ -> fail NumeroNotaInvalido

let createNotaSolicitadoEnvio dataEmissao rps =
    { DataEmissao = dataEmissao; Rps = rps }



module RpsMapper =

    let createNumeroRpsFromDb numero =
        match numero with
        | Some v when v > 0 -> succeed v
        | None -> fail NumeroRpsVazio
        | _ -> fail NumeroRpsInvalido

    let createSerieRpsFromDb serie =
        match serie with
        | Some v when not (String.IsNullOrWhiteSpace v) -> succeed v
        | Some _ -> fail SerieRpsInvalida
        | None -> fail SerieRpsVazia

    let createTipoRpsFromDb tipo =
        match tipo with
        | Some tipo ->
            match tipo with
            | 1 -> succeed Rps
            | 2 -> succeed NotaFiscalConjugadaMista
            | 3 -> succeed Cupom
            | _ -> fail TipoRpsInvalido
        | None -> fail TipoRpsVazio

    let createRps numero serie tipo =
        { Numero = numero
          Serie = serie
          Tipo = tipo }

    let createRpsFromDb numero serie tipoRpsId =
        createRps <!> createNumeroRpsFromDb numero
        <*> createSerieRpsFromDb serie
        <*> createTipoRpsFromDb tipoRpsId

let createNotaSolicitadaEnvioFromDb dataEmissao numeroRps serieRps tipoRps =
    createNotaSolicitadoEnvio <!> createDataEmissaoFromDb dataEmissao
    <*> RpsMapper.createRpsFromDb numeroRps serieRps tipoRps

module ErroComunicacaoMapper =
    let createCodigoErroFromDb codigo =
        match codigo with
        | v when not (String.IsNullOrWhiteSpace v) -> succeed v
        | _ -> fail CodigoErroComunicacaoVazio

    let createMensagemErroComunicacaoFromDb mensagem =
        match mensagem with
        | v when not (String.IsNullOrWhiteSpace v) -> succeed v
        | _ -> fail MensagemAlertaVazia

    let createCorrecaoErroComunicacaoFromDb correcao =
        match correcao with
        | Some v when not (String.IsNullOrWhiteSpace v) -> correcao |> succeed
        | Some _ -> fail CorrecaoInvalida
        | None -> succeed None

    let createErroComunicacao codigoMensagem mensagem correcao =
        { CodigoMensagemAlerta = codigoMensagem
          Mensagem = mensagem
          Correcao = correcao }

    let createErroComunicacaoFromDb (erroComunicacao: dbo.ErroComunicacao) =
        createErroComunicacao <!> createCodigoErroFromDb erroComunicacao.CodigoErro
        <*> createMensagemErroComunicacaoFromDb erroComunicacao.Mensagem
        <*> createCorrecaoErroComunicacaoFromDb erroComunicacao.Correcao

    let createErrosComunicacoesFromDb (errosComunicacoes: dbo.ErroComunicacao list) =
        let anyErrorFailedToConvert err =
            match err with
            | Failure _ -> true
            | _ -> false

        match errosComunicacoes with
        | [] -> fail EmptyErroComunicacaoFromDb
        | errs ->
            match List.map createErroComunicacaoFromDb errs with
            | messages when messages |> List.exists anyErrorFailedToConvert -> fail FailConvertMensagemRetornoFromDb
            | messages ->
                messages
                |> List.map (function
                    | Success(value, _) -> [ value ]
                    | Failure _ -> [])
                |> List.concat
                |> succeed

module AguardandoAutorizacaoMapper =
    let mapToNotaFiscalStatus status = (status >> AguardandoAutorizacao)

    let createNotaAguardandoEnvioStatusFromDb dataEmissao numeroRps serieRps tipoRps =
        mapToNotaFiscalStatus AguardandoEnvio
        <!> createNotaSolicitadaEnvioFromDb dataEmissao numeroRps serieRps tipoRps

    let createNotaAguardandoAutorizacaoStatusFromDb dataEmissao numeroRps serieRps tipoRps =
        mapToNotaFiscalStatus AguardandoAutorizacaoPrefeitura
        <!> createNotaSolicitadaEnvioFromDb dataEmissao numeroRps serieRps tipoRps

module AutorizadaMapper =
    let createNotaAutorizada notaSolicitadaEnvioData numeroNota =
        (notaSolicitadaEnvioData, numeroNota) |> Autorizada

    let createNotaAutorizadaStatusFromDb dataEmissao numeroNota numeroRps serieRps tipoRps =
        createNotaAutorizada
        <!> (createNotaSolicitadaEnvioFromDb dataEmissao numeroRps serieRps tipoRps)
        <*> createNumeroNotaFromDb numeroNota

module ErroAutorizacaoMapper =
    let createNotaErroAutorizacao notaSolicitadaEnvioData errosComunicacoes =
        (notaSolicitadaEnvioData, errosComunicacoes) |> ErroAutorizacao

    let createNotaErroAutorizacaoStatusFromDb dataEmissao numeroRps serieRps tipoRps errosComunicacoes =
        createNotaErroAutorizacao
        <!> (createNotaSolicitadaEnvioFromDb dataEmissao numeroRps serieRps tipoRps)
        <*> ErroComunicacaoMapper.createErrosComunicacoesFromDb errosComunicacoes

module CanceladaMapper =
    let createCodigoCancelamentoFromDb codigo =
        match codigo with
        | Some v when not (v |> String.IsNullOrWhiteSpace) -> succeed v
        | Some _ -> fail CodigoCancelamentoInvalido
        | None -> fail CodigoCancelamentoVazio

    let createNotaCancelada notaSolicitadaEnvioData numeroNota codigoCancelamento =
        (notaSolicitadaEnvioData, numeroNota, codigoCancelamento) |> Cancelada

    let createNotaCanceladaStatusFromDb dataEmissao numeroNota numeroRps serieRps tipoRps codigoCancelamento =
        createNotaCancelada
        <!> (createNotaSolicitadaEnvioFromDb dataEmissao numeroRps serieRps tipoRps)
        <*> createNumeroNotaFromDb numeroNota
        <*> createCodigoCancelamentoFromDb codigoCancelamento

module SolicitandoCancelamentoMapper =
    let createNotaSolicitandoCancelamento notaSolicitadaEnvioData numeroNota codigoCancelamento =
        (notaSolicitadaEnvioData, numeroNota, codigoCancelamento)
        |> SolicitandoCancelamento

    let createNotaSolicitandoCancelamentoStatusFromDb
        dataEmissao
        numeroNota
        codigoCancelamento
        numeroRps
        serieRps
        tipoRps
        =
        createNotaSolicitandoCancelamento
        <!> (createNotaSolicitadaEnvioFromDb dataEmissao numeroRps serieRps tipoRps)
        <*> createNumeroNotaFromDb numeroNota
        <*> CanceladaMapper.createCodigoCancelamentoFromDb codigoCancelamento

module FalhaCancelamentoMapper =
    let createNotaFalhaCancelamento notaSolicitadaEnvioData numeroNota codigoCancelamento errosComunicacoes =
        (notaSolicitadaEnvioData, numeroNota, codigoCancelamento, errosComunicacoes)
        |> FalhaCancelamento

    let createNotaFalhaCancelamentoStatusFromDb
        dataEmissao
        numeroNota
        codigoCancelamento
        numeroRps
        serieRps
        tipoRps
        errosComunicacoes
        =
        createNotaFalhaCancelamento
        <!> (createNotaSolicitadaEnvioFromDb dataEmissao numeroRps serieRps tipoRps)
        <*> createNumeroNotaFromDb numeroNota
        <*> CanceladaMapper.createCodigoCancelamentoFromDb codigoCancelamento
        <*> ErroComunicacaoMapper.createErrosComunicacoesFromDb errosComunicacoes

let createNotaStatusFromDb
    discriminator
    numeroRps
    serieRps
    tipoRps
    dataEmissao
    numeroNota
    errosComunicacoes
    codigoCancelamento
    =
    match discriminator with
    | nameof (Pendente) -> succeed Pendente
    | nameof (AguardandoEnvio) ->
        AguardandoAutorizacaoMapper.createNotaAguardandoEnvioStatusFromDb dataEmissao numeroRps serieRps tipoRps
    | nameof (AguardandoAutorizacaoPrefeitura) ->
        AguardandoAutorizacaoMapper.createNotaAguardandoAutorizacaoStatusFromDb dataEmissao numeroRps serieRps tipoRps
    | nameof (Autorizada) ->
        AutorizadaMapper.createNotaAutorizadaStatusFromDb dataEmissao numeroNota numeroRps serieRps tipoRps
    | nameof (ErroAutorizacao) ->
        ErroAutorizacaoMapper.createNotaErroAutorizacaoStatusFromDb
            dataEmissao
            numeroRps
            serieRps
            tipoRps
            errosComunicacoes
    | nameof (Cancelada) ->
        CanceladaMapper.createNotaCanceladaStatusFromDb
            dataEmissao
            numeroNota
            numeroRps
            serieRps
            tipoRps
            codigoCancelamento
    | nameof (SolicitandoCancelamento) ->
        SolicitandoCancelamentoMapper.createNotaSolicitandoCancelamentoStatusFromDb
            dataEmissao
            numeroNota
            codigoCancelamento
            numeroRps
            serieRps
            tipoRps
    | nameof (FalhaCancelamento) ->
        FalhaCancelamentoMapper.createNotaFalhaCancelamentoStatusFromDb
            dataEmissao
            numeroNota
            codigoCancelamento
            numeroRps
            serieRps
            tipoRps
            errosComunicacoes
    | _ -> fail InvalidStatus
