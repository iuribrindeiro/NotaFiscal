module NotaFiscal.Domain.Dto.StatusDto

open System
open NotaFiscal.Domain
open NotaFiscal.Domain.Entities.NotaFiscalStatus
open NotaFiscal.Domain.Entities.NotaFiscalStatus.NotaFiscalStatusUtils

type StatusDto =
    { Discriminator: string
      NumeroLote: int
      NumeroProtocolo: string
      DataCancelamento: DateTime option
      NumeroNota: string
      DataEmissao: DateTime
      ErrosComunicacao: ErroComunicacaoDto list option
      Rps: RpsDto option
      CodigoCancelamento: string }

and ErroComunicacaoDto =
    { Id: Guid
      CodigoMensagemAlerta: string
      Mensagem: string
      Correcao: string }

and RpsDto = { Numero: int; Serie: string; Tipo: int }


let fromRpsDomain (rps: Rps) : RpsDto =
    { Numero = rps.Numero |> PositiveInt.mapToValue
      Serie = rps.Serie |> StrMax5.mapToValue
      Tipo = rps.Tipo |> TipoRps.mapValue }

let fromErrosComunicacaoDomain
    (errorsComunicacao: ErroComunicacao list)
    : ErroComunicacaoDto list
    =
    errorsComunicacao
    |> List.map (fun errCmn ->
        { Id = errCmn.Id
          CodigoMensagemAlerta = errCmn.CodigoMensagemAlerta
          Mensagem = errCmn.Mensagem
          Correcao = errCmn.Correcao |> Option.defaultValue null })

let fromStatusDomain (status: NotaFiscalServicoStatus) : StatusDto =
    match status with
    | Pendente ->
        { Discriminator = nameof Pendente
          NumeroLote = 0
          NumeroProtocolo = null
          NumeroNota = null
          DataEmissao = None
          ErrosComunicacao = None
          Rps = None
          CodigoCancelamento = null
          DataCancelamento = None }
    | SolicitandoEmissao(numeroLote, dataEmissao, rps) ->
        { Discriminator = nameof SolicitandoEmissao
          NumeroLote = numeroLote |> PositiveInt.mapToValue
          NumeroProtocolo = null
          NumeroNota = null
          DataEmissao = dataEmissao |> PastDate.mapToValue |> Some
          ErrosComunicacao = None
          Rps = fromRpsDomain rps |> Some
          CodigoCancelamento = null
          DataCancelamento = None }
    | AguardandoAutorizacao(numeroLote, protocolo, dataEmissao, rps) ->
        { Discriminator = nameof AguardandoAutorizacao
          NumeroLote = numeroLote |> PositiveInt.mapToValue
          NumeroProtocolo = protocolo
          NumeroNota = null
          DataEmissao = dataEmissao |> PastDate.mapToValue |> Some
          ErrosComunicacao = None
          Rps = fromRpsDomain rps |> Some
          CodigoCancelamento = null
          DataCancelamento = None }
    | Autorizada(dataEmissao, rps, numeroNota) ->
        { Discriminator = nameof Autorizada
          NumeroLote = 0
          NumeroProtocolo = null
          NumeroNota = numeroNota
          DataEmissao = dataEmissao |> PastDate.mapToValue |> Some
          ErrosComunicacao = None
          Rps = fromRpsDomain rps |> Some
          CodigoCancelamento = null
          DataCancelamento = None }
    | ErroAutorizacao(dataEmissao, rps, errosComunicacao) ->
        { Discriminator = nameof ErroAutorizacao
          NumeroLote = 0
          NumeroProtocolo = null
          NumeroNota = null
          DataEmissao = dataEmissao |> PastDate.mapToValue |> Some
          ErrosComunicacao = fromErrosComunicacaoDomain errosComunicacao |> Some
          Rps = fromRpsDomain rps |> Some
          CodigoCancelamento = null
          DataCancelamento = None }
    | SolicitandoCancelamento(dataEmissao, rps, numeroNota, codigoCancelamento) ->
        { Discriminator = nameof SolicitandoCancelamento
          NumeroLote = 0
          NumeroProtocolo = null
          NumeroNota = numeroNota
          DataEmissao = dataEmissao |> PastDate.mapToValue |> Some
          ErrosComunicacao = None
          Rps = fromRpsDomain rps |> Some
          CodigoCancelamento = codigoCancelamento |> NotEmptyStr.mapToValue
          DataCancelamento = None }
    | Cancelada(dataEmissao,
                rps,
                numeroNota,
                codigoCancelamento,
                dataCancelamento) ->
        { Discriminator = nameof Cancelada
          NumeroLote = 0
          NumeroProtocolo = null
          NumeroNota = numeroNota
          DataEmissao = dataEmissao |> PastDate.mapToValue |> Some
          ErrosComunicacao = None
          Rps = fromRpsDomain rps |> Some
          CodigoCancelamento = codigoCancelamento |> NotEmptyStr.mapToValue
          DataCancelamento = Some dataCancelamento }

let toRpsDomain (rpsDto: RpsDto) : OperationResult<Rps, RpsErrors> =
    createRps rpsDto.Numero rpsDto.Serie rpsDto.Tipo

let toErroComunicacaoDomain
    (erroComunicacaoDto: ErroComunicacaoDto)
    : ErroComunicacao
    =
    { Id = erroComunicacaoDto.Id
      Correcao = strToOptionStr erroComunicacaoDto.Correcao
      Mensagem = erroComunicacaoDto.Mensagem
      CodigoMensagemAlerta = erroComunicacaoDto.CodigoMensagemAlerta }

let defaultRps =
    { Numero = 1; Serie = "1"; Tipo = 1 }

let toStatusDomain (statusDto: StatusDto) =
    match statusDto.Discriminator with
    | nameof Pendente -> Pendente
    | nameof SolicitandoEmissao ->
        createStatusSolicitandoEmissao
            statusDto.NumeroLote
            statusDto.DataEmissao
            (toRpsDomain (statusDto.Rps |> Option.defaultValue defaultRps))
    | nameof AguardandoAutorizacao ->
        createStatusAguardandoAutorizacao
        <| statusDto.NumeroLote
        <| statusDto.NumeroProtocolo
        <| statusDto.DataEmissao.GetValueOrDefault DateTime.MinValue
        <| toRpsDomain (statusDto.Rps |> Option.defaultValue defaultRps)
    | nameof Autorizada ->
        createStatusAutorizada
        <| statusDto.DataEmissao.GetValueOrDefault DateTime.MinValue
        <| toRpsDomain statusDto.Rps
        <| statusDto.NumeroNota
    | nameof SolicitandoCancelamento ->
        createStatusSolicitandoCancelamento
        <| statusDto.NumeroNota
        <| statusDto.DataEmissao.GetValueOrDefault DateTime.MinValue
        <| toRpsDomain statusDto.Rps
        <| statusDto.CodigoCancelamento
    | nameof Cancelada ->
        createStatusCancelada
        <| statusDto.DataEmissao.GetValueOrDefault DateTime.MinValue
        <| toRpsDomain statusDto.Rps
        <| statusDto.NumeroNota
        <| statusDto.CodigoCancelamento
        <| statusDto.DataCancelamento.GetValueOrDefault DateTime.MinValue

    | nameof ErroAutorizacao ->
        createStatusErroAutorizacao
        <| statusDto.DataEmissao.GetValueOrDefault DateTime.MinValue
        <| toRpsDomain statusDto.Rps
        <| List.map toErroComunicacaoDomain statusDto.ErrosComunicacao

    | _ -> failwith "Status da nota inválido"
