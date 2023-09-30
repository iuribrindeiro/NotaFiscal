module NotaFiscal.Domain.Entities.NotaFiscalStatus

open System
open NotaFiscal.Domain

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
    { CodigoMensagemAlerta: string; Mensagem: string; Correcao: string option }

and Rps = { Numero: PositiveInt; Serie: StrMax5; Tipo: TipoRps }

and CodigoCancelamento = NotEmptyStr
and DataEmissao = PastDate
and DataCancelamento = DateTime
and NumeroNota = string
and NumeroProtocolo = string
and NumeroLote = PositiveInt
and CodigoErro = string
and Correcao = string


module NotaFiscalStatusUtils =

    let createRps numero serie tipo =
        let createRps' numero serie tipo =
            { Numero = numero; Serie = serie; Tipo = tipo }

        let createNumeroRps numero =
            let mapNumeroRpsErrs err =
                match err with
                | NumberError.Missing -> RpsErrors.NumeroIsRequired
                | MustBePositive -> RpsErrors.NumeroMustBePositive
                | x ->
                    x.GetType().Name
                    |> failwithf "%s não esperado para o campo rua"

            PositiveInt.create numero |> mapFailuresR mapNumeroRpsErrs


        let createSerieRps serie =
            let mapSerieRpsErrs err =
                match err with
                | StringError.Missing -> RpsErrors.SerieIsRequired
                | MustNotBeLongerThan _ -> RpsErrors.TipoInvalid
                | x ->
                    x.GetType().Name
                    |> failwithf "%s não esperado para o campo rua"

            StrMax5.create serie |> mapFailuresR mapSerieRpsErrs


        let createTipoRps tipo =
            let mapTipoRpsErrs (err: EnumError<int>) =
                match err with
                | EnumError.Invalid _ -> RpsErrors.TipoInvalid

            TipoRps.create tipo |> mapFailuresR mapTipoRpsErrs


        createRps' <!> createNumeroRps numero
        <*> createSerieRps serie
        <*> createTipoRps tipo

    let createStatusAguardandoAutorizacao
        numeroLote
        numeroProtocolo
        dataEmissao
        rps
        =
        (numeroLote, numeroProtocolo, dataEmissao, rps)
        |> AguardandoAutorizacao

    let createStatusAutorizada dataEmissao rps numeroNota =
        (dataEmissao, rps, numeroNota) |> Autorizada

    let createStatusErroAutorizacao dataEmissao rps errosComunicacoes =
        (dataEmissao, rps, errosComunicacoes) |> ErroAutorizacao

    let createStatusSolicitandoEmissao
        (numeroLote: int)
        (dataEmissao: DateTime)
        (rps: OperationResult<Rps, RpsErrors>)
        : OperationResult<NotaFiscalServicoStatus, StatusNotaInvalidoErrs>
        =
        let createStatusSolicitandoEmissao' numeroLote dataEmissao rps =
            (numeroLote, dataEmissao, rps) |> SolicitandoEmissao

        let createNumeroLote =
            let mapErrs =
                function
                | MustBePositive ->
                    SolicitandoEmissaoErrors.NumeroLoteMustBePositive
                | x ->
                    x.GetType().Name
                    |> failwithf "%s não esperado para o campo numero lote"

            PositiveInt.create numeroLote |> mapFailuresR mapErrs

        let createDtEmissao =
            let mapErrs =
                function
                | DateError.Missing ->
                    SolicitandoEmissaoErrors.DataEmissaoIsRequired
                | DateError.MustBePast ->
                    SolicitandoEmissaoErrors.DataEmissaoMustBeLessThanNow

            PastDate.create dataEmissao |> mapFailuresR mapErrs

        let createRps' =
            rps |> mapFailuresR SolicitandoEmissaoErrors.RpsInvalido

        createStatusSolicitandoEmissao' <!> createNumeroLote
        <*> createDtEmissao
        <*> createRps'
        |> mapFailuresR SolicitandoEmissaoErrors

    let createStatusSolicitandoCancelamento
        numeroNota
        dataEmissao
        rps
        codigoCancelamento
        =
        (dataEmissao, rps, numeroNota, codigoCancelamento)
        |> SolicitandoCancelamento

    let createStatusCancelada
        dataEmissao
        rps
        numeroNota
        codigoCancelamento
        dataCancelamento
        =
        (dataEmissao, rps, numeroNota, codigoCancelamento, dataCancelamento)
        |> Cancelada
