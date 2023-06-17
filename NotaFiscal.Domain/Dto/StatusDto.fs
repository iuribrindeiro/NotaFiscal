module NotaFiscal.Domain.Dto.StatusDto

open System
open NotaFiscal.Domain.NotaFiscalStatus
open NotaFiscal.Domain.Rop

type StatusDto =
    { Discriminator: string
      NumeroLote: string
      NumeroProtocolo: string
      DataCancelamento: DateTime Nullable
      NumeroNota: string
      DataEmissao: DateTime Nullable
      ErrosComunicacao: ErroComunicacaoDto list
      Rps: RpsDto
      CodigoCancelamento: string }

and ErroComunicacaoDto =
    { Id: Guid
      CodigoMensagemAlerta: string
      Mensagem: string
      Correcao: string }
    
and RpsDto = { Numero: int; Serie: string; Tipo: int }


let fromRpsDomain (rps: Rps) : RpsDto =
    let tipoRpsDto =
        match rps.Tipo with
        | Rps -> 1
        | NotaFiscalConjugadaMista -> 2
        | Cupom -> 3

    { Numero = rps.Numero; Serie = rps.Serie; Tipo = tipoRpsDto }

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
          NumeroLote = null
          NumeroProtocolo = null
          NumeroNota = null
          DataEmissao = Nullable()
          ErrosComunicacao = []
          Rps = Unchecked.defaultof<RpsDto>
          CodigoCancelamento = null
          DataCancelamento = Nullable() }
    | SolicitandoEmissao(numeroLote, dataEmissao, rps) ->
        { Discriminator = nameof SolicitandoEmissao
          NumeroLote = numeroLote
          NumeroProtocolo = null
          NumeroNota = null
          DataEmissao = Nullable dataEmissao
          ErrosComunicacao = []
          Rps = fromRpsDomain rps
          CodigoCancelamento = null
          DataCancelamento = Nullable() }
    | AguardandoAutorizacao(numeroLote, protocolo, dataEmissao, rps) ->
        { Discriminator = nameof AguardandoAutorizacao
          NumeroLote = numeroLote
          NumeroProtocolo = protocolo
          NumeroNota = null
          DataEmissao = Nullable dataEmissao
          ErrosComunicacao = []
          Rps = fromRpsDomain rps
          CodigoCancelamento = null
          DataCancelamento = Nullable() }
    | Autorizada(dataEmissao, rps, numeroNota) ->
        { Discriminator = nameof Autorizada
          NumeroLote = null
          NumeroProtocolo = null
          NumeroNota = numeroNota
          DataEmissao = Nullable dataEmissao
          ErrosComunicacao = []
          Rps = fromRpsDomain rps
          CodigoCancelamento = null
          DataCancelamento = Nullable() }
    | ErroAutorizacao(dataEmissao, rps, errosComunicacao) ->
        { Discriminator = nameof ErroAutorizacao
          NumeroLote = null
          NumeroProtocolo = null
          NumeroNota = null
          DataEmissao = Nullable dataEmissao
          ErrosComunicacao =
            fromErrosComunicacaoDomain errosComunicacao
          Rps = fromRpsDomain rps
          CodigoCancelamento = null
          DataCancelamento = Nullable() }
    | SolicitandoCancelamento(dataEmissao,
                              rps,
                              numeroNota,
                              codigoCancelamento) ->
        { Discriminator = nameof SolicitandoCancelamento
          NumeroLote = null
          NumeroProtocolo = null
          NumeroNota = numeroNota
          DataEmissao = Nullable dataEmissao
          ErrosComunicacao = []
          Rps = fromRpsDomain rps
          CodigoCancelamento = codigoCancelamento
          DataCancelamento = Nullable() }
    | Cancelada(dataEmissao,
                rps,
                numeroNota,
                codigoCancelamento,
                dataCancelamento) ->
        { Discriminator = nameof Cancelada
          NumeroLote = null
          NumeroProtocolo = null
          NumeroNota = numeroNota
          DataEmissao = Nullable dataEmissao
          ErrosComunicacao = []
          Rps = fromRpsDomain rps
          CodigoCancelamento = codigoCancelamento
          DataCancelamento = Nullable dataCancelamento }

let toTipoRpsDto tipoRps =
    match tipoRps with
    | 1 -> Rps
    | 2 -> NotaFiscalConjugadaMista
    | _ -> Cupom
let toRpsDomain (rpsDto: RpsDto): Rps =
    { Numero = rpsDto.Numero; Serie = rpsDto.Serie; Tipo = toTipoRpsDto rpsDto.Tipo }
    
let toErroComunicacaoDomain (erroComunicacaoDto: ErroComunicacaoDto): ErroComunicacao =
    { Id = erroComunicacaoDto.Id
      Correcao = strToOptionStr erroComunicacaoDto.Correcao
      Mensagem = erroComunicacaoDto.Mensagem
      CodigoMensagemAlerta = erroComunicacaoDto.CodigoMensagemAlerta }
        
let toStatusDomain (statusDto: StatusDto) =
    match statusDto.Discriminator with
    | nameof Pendente -> Pendente
    | nameof SolicitandoEmissao ->
        createStatusSolicitandoEmissao
            <| statusDto.NumeroLote
            <| statusDto.DataEmissao.GetValueOrDefault DateTime.MinValue
            <| toRpsDomain statusDto.Rps
    | nameof AguardandoAutorizacao ->
        createStatusAguardandoAutorizacao
            <| statusDto.NumeroLote
            <| statusDto.NumeroProtocolo
            <| statusDto.DataEmissao.GetValueOrDefault DateTime.MinValue
            <| toRpsDomain statusDto.Rps
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
