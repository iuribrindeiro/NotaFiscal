module NotaFiscal.Data.Mappers.NotaFiscalStatusMapper

open NotaFiscal.Domain.ApplicationErrors
open NotaFiscal.Domain.NotaFiscalStatus
open NotaFiscal.Data.DbAutoGen
open System



let toDataEmissaoDomain (dataEmissao: DateTime option) =
    match dataEmissao with
    | Some dataEmissao -> succeed dataEmissao
    | None -> fail DataEmissaoVazia

let toNumeroNotaDomain (numeroNota: int option) =
    match numeroNota with
    | Some v when v > 0 -> succeed v
    | _ -> fail NumeroNotaInvalido

let toNumeroProtocoloDomain serie =
    match serie with
    | Some v when not (String.IsNullOrWhiteSpace v) -> succeed v
    | _ -> fail NumeroProtocoloVazio

let toNumeroLoteDomain serie =
    match serie with
    | Some v when not (String.IsNullOrWhiteSpace v) -> succeed v
    | _ -> fail NumeroProtocoloVazio


module RpsMapper =

    let toRpsDomain numero serie tipoRpsId =
        let toNumeroRpsDomain numero =
            match numero with
            | Some v when v > 0 -> succeed v
            | None -> fail NumeroRpsVazio
            | _ -> fail NumeroRpsInvalido

        let toSerieRpsDomain serie =
            match serie with
            | Some v when not (String.IsNullOrWhiteSpace v) -> succeed v
            | Some _ -> fail SerieRpsInvalida
            | None -> fail SerieRpsVazia

        let toTipoRpsDomain tipo =
            match tipo with
            | Some tipo ->
                match tipo with
                | 1 -> succeed Rps
                | 2 -> succeed NotaFiscalConjugadaMista
                | 3 -> succeed Cupom
                | _ -> fail TipoRpsInvalido
            | None -> fail TipoRpsVazio

        createRps
            <!> toNumeroRpsDomain numero
            <*> toSerieRpsDomain serie
            <*> toTipoRpsDomain tipoRpsId


    let toTipoRpsDb rps =
        match rps.Tipo with
        | Rps -> 1
        | NotaFiscalConjugadaMista -> 2
        | Cupom -> 3

module ErroComunicacaoMapper =
    let toCodigoErroDomain codigo =
        match codigo with
        | v when not (String.IsNullOrWhiteSpace v) -> succeed v
        | _ -> fail CodigoErroComunicacaoVazio

    let toMensagemErroDomain mensagem =
        match mensagem with
        | v when not (String.IsNullOrWhiteSpace v) -> succeed v
        | _ -> fail MensagemAlertaVazia

    let toCorrecaoErroDomain correcao =
        match correcao with
        | Some v when not (String.IsNullOrWhiteSpace v) -> correcao |> succeed
        | Some _ -> fail CorrecaoInvalida
        | None -> succeed None

    let toErroComunicacaoDomain' id codigoMensagem mensagem correcao =
        { Id = id
          CodigoMensagemAlerta = codigoMensagem
          Mensagem = mensagem
          Correcao = correcao }

    let toErroComunicacaoDb notaFiscalId (erroComunicacao: ErroComunicacao) =
        { dbo.ErroComunicacao.CodigoErro = erroComunicacao.CodigoMensagemAlerta
          dbo.ErroComunicacao.Mensagem = erroComunicacao.Mensagem
          dbo.ErroComunicacao.Correcao = erroComunicacao.Correcao
          dbo.ErroComunicacao.Id = erroComunicacao.Id
          dbo.ErroComunicacao.NotaFiscalServicoId = notaFiscalId }

    let toErroComunicacaoDomain (erroComunicacao: dbo.ErroComunicacao) =
        toErroComunicacaoDomain' erroComunicacao.Id
        <!> toCodigoErroDomain erroComunicacao.CodigoErro
        <*> toMensagemErroDomain erroComunicacao.Mensagem
        <*> toCorrecaoErroDomain erroComunicacao.Correcao

    let toErrosComunicacaoDomain (errosComunicacoesDb: dbo.ErroComunicacao list) =
        match errosComunicacoesDb with
        | [] -> fail EmptyErroComunicacaoFromDb
        | _ ->
            match List.map toErroComunicacaoDomain errosComunicacoesDb with
            | messages when messages |> hasAnyFailure -> fail FailConvertMensagemRetornoFromDb
            | messages -> mapSuccessResults messages |> succeed

module AguardandoAutorizacaoMapper =

    let toAutorizandoDomain (dataEmissao: DateTime option) numeroLote numeroProtocolo numeroRps serieRps tipoRps =
        createStatusAguardandoAutorizacao
        <!> toNumeroLoteDomain numeroLote
        <*> toNumeroProtocoloDomain numeroProtocolo
        <*> toDataEmissaoDomain dataEmissao
        <*> RpsMapper.toRpsDomain numeroRps serieRps tipoRps


module ErroAutorizacaoMapper =
    let toErroAutorizacaoDomain dataEmissao numeroRps serieRps tipoRps errosComunicacoes =
        createStatusErroAutorizacao
        <!> toDataEmissaoDomain dataEmissao
        <*> RpsMapper.toRpsDomain numeroRps serieRps tipoRps
        <*> ErroComunicacaoMapper.toErrosComunicacaoDomain errosComunicacoes

module AutorizadaMapper =
    let toAutorizadaDomain dataEmissao numeroNota numeroRps serieRps tipoRps =
        createStatusAutorizada <!> toDataEmissaoDomain dataEmissao
        <*> RpsMapper.toRpsDomain numeroRps serieRps tipoRps
        <*> toNumeroNotaDomain numeroNota

module CanceladaMapper =
    let toCodigoCancelamentoDomain codigo =
        match codigo with
        | Some v when not (v |> String.IsNullOrWhiteSpace) -> succeed v
        | Some _ -> fail CodigoCancelamentoInvalido
        | None -> fail CodigoCancelamentoVazio

    let toCanceladaDomain dataEmissao numeroNota numeroRps serieRps tipoRps codigoCancelamento =
        createStatusCancelada <!> toDataEmissaoDomain dataEmissao
        <*> (RpsMapper.toRpsDomain numeroRps serieRps tipoRps)
        <*> toNumeroNotaDomain numeroNota
        <*> toCodigoCancelamentoDomain codigoCancelamento

let toNotaFiscalStatusDomain
    discriminator
    numeroRps
    serieRps
    tipoRps
    dataEmissao
    numeroNota
    errosComunicacoes
    codigoCancelamento
    numeroLote
    numeroProtocolo
    =
    match discriminator with
    | nameof (Pendente) -> succeed Pendente
    | nameof (AguardandoAutorizacao) ->
        AguardandoAutorizacaoMapper.toAutorizandoDomain
            dataEmissao
            numeroLote
            numeroProtocolo
            numeroRps
            serieRps
            tipoRps
    | nameof (Autorizada) -> AutorizadaMapper.toAutorizadaDomain dataEmissao numeroNota numeroRps serieRps tipoRps
    | nameof (ErroAutorizacao) ->
        ErroAutorizacaoMapper.toErroAutorizacaoDomain dataEmissao numeroRps serieRps tipoRps errosComunicacoes
    | nameof (Cancelada) ->
        CanceladaMapper.toCanceladaDomain dataEmissao numeroNota numeroRps serieRps tipoRps codigoCancelamento
    | _ -> fail InvalidStatus