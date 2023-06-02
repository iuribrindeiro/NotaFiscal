module NotaFiscal.Data.Mappers.NotaFiscalMapper

open NotaFiscal.Data.DbAutoGen
open NotaFiscal.Domain.NotaFiscalServico
open TomadorMapper
open NotaFiscalStatusMapper
open NotaFiscal.Domain.ApplicationErrors
open NotaFiscal.Domain.Servico.ValoresServico
open NotaFiscal.Domain.Servico.Servico
open NotaFiscal.Domain.NotaFiscalPrimitives
open NotaFiscal.Domain.NotaFiscalStatus
open NotaFiscalStatusMapper.RpsMapper
open NotaFiscalStatusMapper.ErroComunicacaoMapper

let toServicoDomain (nota: dbo.NotaFiscalServico) =
    let valoresServicoResult =
        createValoresServico
            nota.ValoresServicos
            nota.ValoresDeducoes
            nota.ValoresPis
            nota.ValoresCofins
            nota.ValoresInss
            nota.ValoresIr
            nota.ValoresCsll
            nota.ValoresIss
            nota.ValoresIssDiscriminator
            nota.ValoresOutrasRetencoes
            nota.ValoresDescontoCondicionado
            nota.ValoresDescontoIncondicionado
            nota.ValoresAliquota


    createServico
        valoresServicoResult
        nota.ItemListaServico
        nota.CodigoTributacaoMunicipio
        nota.Discriminacao
        nota.MunicipioPrestacaoServico
        nota.CodigoCnae
        nota.ServicoNaturezaOperacaoId
        nota.ServicoRegimeEspecialTributacaoId
        nota.OptanteSimplesNacional
        nota.IncentivadorCultural

let createNotaFiscalWithStatus notaFiscal status =
    { notaFiscal with Status = status }

let toNotafiscalDomain
    (nota: dbo.NotaFiscalServico)
    (tomadorDb: dbo.Tomador option)
    (enderecoDb: dbo.Endereco option)
    (contato: dbo.Contato option)
    (errosComunicacoes: dbo.ErroComunicacao list)
    =
    let servicoResult = toServicoDomain nota

    let tomadorResult =
        toTomadorDomain tomadorDb enderecoDb contato nota.TomadorDiscriminator

    let statusResult =
        toNotaFiscalStatusDomain
            nota.Discriminator
            nota.RpsNumero
            nota.RpsSerie
            nota.TipoRpsId
            nota.DataEmissao
            nota.NumeroNota
            errosComunicacoes
            nota.CodigoCancelamento
            nota.NumeroLote
            nota.NumeroProtocolo
        |> mapFailuresR NotaFiscalStatusInvalido


    createNotaFiscalWithStatus
    <!> createNotaFiscalServico
        nota.Id
        tomadorResult
        servicoResult
    <*> statusResult

let mapNotaFiscalTable
    status
    dataEmissao
    numeroLote
    numeroProtocolo
    (rps: Rps option)
    numeroNota
    codigoCancelamento
    (nota: NotaFiscalServico)
    (tomadorDb: dbo.Tomador option)
    valorIss
    issDiscriminator
    =
    let codigoCnae =
        nota.Servico.CodigoCnae
        |> StrMax7.mapToValueOptional


    let codigoTributacao =
        nota.Servico.CodigoTributacaoMunicipio
        |> StrMax20.mapToValueOption

    let discriminacao =
        nota.Servico.Discriminacao
        |> StrMax2000.mapToValue

    let itemListaServico =
        nota.Servico.ItemListaServico
        |> StrMax7.mapToValue

    let municipioPrestacaoServico =
        nota.Servico.CodigoMunicipioPrestacao
        |> StrMax7.mapToValue


    { dbo.NotaFiscalServico.Id = nota.Id
      dbo.NotaFiscalServico.Discriminator = status
      dbo.NotaFiscalServico.CodigoCancelamento = codigoCancelamento
      dbo.NotaFiscalServico.CodigoCnae = codigoCnae
      dbo.NotaFiscalServico.CodigoTributacaoMunicipio = codigoTributacao
      dbo.NotaFiscalServico.DataEmissao = dataEmissao
      dbo.NotaFiscalServico.Discriminacao = discriminacao
      dbo.NotaFiscalServico.IncentivadorCultural = nota.Servico.IncentivadorCultural
      dbo.NotaFiscalServico.ItemListaServico = itemListaServico
      dbo.NotaFiscalServico.MunicipioPrestacaoServico = municipioPrestacaoServico
      dbo.NotaFiscalServico.NumeroLote = numeroLote
      dbo.NotaFiscalServico.NumeroNota = numeroNota
      dbo.NotaFiscalServico.NumeroProtocolo = numeroProtocolo
      dbo.NotaFiscalServico.OptanteSimplesNacional = nota.Servico.OptanteSimplesNacional
      dbo.NotaFiscalServico.RpsNumero = rps |> Option.map (fun x -> x.Numero)
      dbo.NotaFiscalServico.RpsSerie = rps |> Option.map (fun x -> x.Serie)
      dbo.NotaFiscalServico.TipoRpsId = rps |> Option.map toTipoRpsDb
      dbo.NotaFiscalServico.ServicoNaturezaOperacaoId = NaturezaOperacao.mapBack nota.Servico.NaturezaOperacao
      dbo.NotaFiscalServico.ServicoRegimeEspecialTributacaoId = RegimeEspecialTributacao.mapBack nota.Servico.RegimeEspecialTributacao
      dbo.NotaFiscalServico.TomadorDiscriminator = toTomadorDiscriminatorDb nota.Tomador
      dbo.NotaFiscalServico.TomadorId = tomadorDb |> Option.map (fun x -> x.Id)
      dbo.NotaFiscalServico.ValoresAliquota = nota.Servico.Valores.Aliquota |> Percentage.mapToValueOptional
      dbo.NotaFiscalServico.ValoresCofins = nota.Servico.Valores.Cofins |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresCsll = nota.Servico.Valores.Csll |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresDeducoes = nota.Servico.Valores.Deducoes |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresDescontoCondicionado = nota.Servico.Valores.DescontoCondicionado |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresDescontoIncondicionado = nota.Servico.Valores.DescontoIncondicionado |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresInss = nota.Servico.Valores.Inss |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresIr = nota.Servico.Valores.Ir |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresIss = valorIss
      dbo.NotaFiscalServico.ValoresIssDiscriminator = issDiscriminator
      dbo.NotaFiscalServico.ValoresOutrasRetencoes = nota.Servico.Valores.OutrasRetencoes |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresPis = nota.Servico.Valores.Pis |> Dinheiro.mapToValueOptional
      dbo.NotaFiscalServico.ValoresServicos = nota.Servico.Valores.Servicos |> Dinheiro.mapToValue }



let toNotaDbByStatus nota =
    match nota.Status with
    | Pendente ->
        mapNotaFiscalTable (nameof (Pendente)) None None None None None None
    | AguardandoAutorizacao(numeroLote, numeroProtocolo, dataEmissao, rps) ->
        mapNotaFiscalTable (nameof (AguardandoAutorizacao))
        <| (Some dataEmissao)
        <| (Some numeroLote)
        <| (Some numeroProtocolo)
        <| (Some rps)
        <| None
        <| None
    | Autorizada(dataEmissao, rps, numeroNota) ->
        mapNotaFiscalTable (nameof (Autorizada))
        <| (Some dataEmissao)
        <| None
        <| None
        <| (Some rps)
        <| (Some numeroNota)
        <| None
    | ErroAutorizacao(dataEmissao, rps, _) ->
        mapNotaFiscalTable (nameof (ErroAutorizacao))
        <| (Some dataEmissao)
        <| None
        <| None
        <| (Some rps)
        <| None
        <| None
    | Cancelada(dataEmissao, rps, numeroNota, codigoCancelamento) ->
        mapNotaFiscalTable (nameof (Cancelada))
        <| (Some dataEmissao)
        <| None
        <| None
        <| (Some rps)
        <| (Some numeroNota)
        <| (Some codigoCancelamento)

let mapErrosComunicacaoToDb nota =
    match nota.Status with
    | ErroAutorizacao(_, _, errosComunicacao) ->
        let toErroComunicacaoDb' =
            toErroComunicacaoDb nota.Id

        errosComunicacao
        |> List.map toErroComunicacaoDb'
    | _ -> []

let toNotaFiscalDb
    (nota: NotaFiscalServico)
    : dbo.NotaFiscalServico *
      dbo.Contato option *
      dbo.Endereco option *
      dbo.Tomador option *
      dbo.ErroComunicacao list
    =
    let (tomador, contato, endereco) =
        toTomadorDb nota.Tomador

    let issDiscriminator =
        match nota.Servico.Valores.Iss with
        | Retido _ -> nameof (Retido)
        | NaoRetido _ -> nameof (NaoRetido)

    let valorIss =
        match nota.Servico.Valores.Iss with
        | Retido v -> v |> Dinheiro.mapToValue
        | NaoRetido v -> v |> Dinheiro.mapToValue

    let toNotaFiscalDb' = toNotaDbByStatus nota

    let notaFiscalDb =
        toNotaFiscalDb' nota tomador valorIss issDiscriminator

    (notaFiscalDb, contato, endereco, tomador, mapErrosComunicacaoToDb nota)
    
let mapNotasFiscaisResults
    (results:
        (dbo.NotaFiscalServico *
        dbo.Tomador option *
        dbo.Endereco option *
        dbo.Contato option *
        dbo.ErroComunicacao option) seq)
    =
    results
    |> Seq.groupBy (fun (nota, _, _, _, _) -> nota.Id)
    |> Seq.map (fun (_, groupedResults) ->
        let nota, tomador, endereco, contato, _ = groupedResults |> Seq.head

        let errosComunicacao =
            groupedResults
            |> Seq.map (fun (_, _, _, _, errosComunicacao) -> errosComunicacao)
            |> Seq.toList
            |> List.choose id

        toNotafiscalDomain nota tomador endereco contato errosComunicacao)