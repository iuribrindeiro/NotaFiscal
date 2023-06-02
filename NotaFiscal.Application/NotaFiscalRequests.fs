module NotaFiscal.WebApplication.NotaFiscalRequests

open System
open NotaFiscal.WebApplication.RequestTypes
open NotaFiscal.Domain.Tomador
open NotaFiscal.Domain.ApplicationErrors
open NotaFiscal.Domain.Servico.Servico
open NotaFiscal.Domain.Servico.ValoresServico
open NotaFiscal.Domain.NotaFiscalServico

type CriarNotaFiscalRequest =
    { Tomador: TomadorRequestDto option; Servico: ServicoRequestDto option }

type SolicitarEmissaoNotaRequest =
    { DataCompetencia: DateTime option; ConfiguracoaEmissaoId: Guid option }

type CriarEmitirNotaRequest =
    { ConfiguracoaEmissaoId: Guid option
      Tomador: TomadorRequestDto option
      Servico: ServicoRequestDto option
      DataCompetencia: DateTime option }

let mapToEnderecoPessoaFisicaDomain (enderecoDto: EnderecoRequestDto option) =
    match enderecoDto with
    | Some dto ->
        createEnderecoPessoaFisica
        <| Guid.NewGuid()
        <| Option.defaultValue "" dto.Rua
        <| Option.defaultValue "" dto.Numero
        <| dto.Complemento
        <| Option.defaultValue "" dto.Bairro
        <| Option.defaultValue "" dto.CodigoMunicipio
        <| Option.defaultValue "" dto.Cep
        |> mapR Some
    | None -> succeed None

let mapToEnderecoPessoaJuridicaDomain (enderecoDto: EnderecoRequestDto option) =
    match enderecoDto with
    | Some dto ->
        createEnderecoPessoaJuridica
        <| Guid.NewGuid()
        <| Option.defaultValue "" dto.Rua
        <| Option.defaultValue "" dto.Numero
        <| dto.Complemento
        <| Option.defaultValue "" dto.Bairro
        <| Option.defaultValue "" dto.CodigoMunicipio
        <| Option.defaultValue "" dto.Cep
    | None -> fail EnderecoIsRequired

let createContatoPessoaFisica (contatoDto: ContatoRequestDto option) =
    match contatoDto with
    | Some dto ->
        createContatoPessoaFisica
        <| Guid.NewGuid()
        <| Option.defaultValue "" dto.Telefone
        <| Option.defaultValue "" dto.Email
        |> mapR Some
    | None -> succeed None

let createContatoPessoaJuridica (contatoDto: ContatoRequestDto option) =
    match contatoDto with
    | Some dto ->
        createContatoPessoaJuridica
        <| Guid.NewGuid()
        <| Option.defaultValue "" dto.Telefone
        <| Option.defaultValue "" dto.Email
    | None -> fail ContatoIsRequired

let mapTomadorDomain tomadorDto =
    match tomadorDto with
    | { Estrangeiro = Some true } -> succeed tomadorEstrangeiro
    | { Cpf = Some cpf
        Endereco = maybeEndereco
        Contato = maybeContato
        InscricaoMunicipal = maybeInscricaoMunicipal } ->
        createTomadorPessoaFisica
        <| Guid.NewGuid()
        <| createCpf cpf
        <| createInscricaoMunicipalTomadorFisico maybeInscricaoMunicipal
        <| mapToEnderecoPessoaFisicaDomain maybeEndereco
        <| createContatoPessoaFisica maybeContato
    | { Cnpj = Some cnpj
        Endereco = maybeEndereco
        Contato = maybeContato
        InscricaoMunicipal = maybeInscricaoMunicipal
        Nome = maybeNome } ->
        createTomadorPessoaJuridica
        <| Guid.NewGuid()
        <| createCnpjTomador cnpj
        <| createInscricaoMunicipalTomadorJuridico maybeInscricaoMunicipal
        <| (Option.defaultValue "" maybeNome
            |> createRazaoSocial)
        <| mapToEnderecoPessoaJuridicaDomain maybeEndereco
        <| createContatoPessoaJuridica maybeContato
    | _ -> PessoaFisica None |> succeed

let mapToServicoDomain (maybeServicoDto: ServicoRequestDto option) =
    let createValoresOrError (maybeValores: ValoresRequestDto option) =
        match maybeValores with
        | Some valores ->
            createValoresServico
            <| Option.defaultValue 0m valores.Servicos
            <| valores.Deducoes
            <| valores.Pis
            <| valores.Cofins
            <| valores.Inss
            <| valores.Ir
            <| valores.Csll
            <| Option.defaultValue 0m valores.ValorIss
            <| Option.defaultValue false valores.IssRetido
            <| valores.OutrasRetencoes
            <| valores.DescontoCondicionado
            <| valores.DescontoIncondicionado
            <| valores.Aliquota
        | None -> fail ValoresIsRequired

    match maybeServicoDto with
    | Some dto ->
        createServico
        <| createValoresOrError dto.Valores
        <| Option.defaultValue "" dto.ItemListaServico
        <| dto.CodigoTributacaoMunicipio
        <| Option.defaultValue "" dto.Discriminacao
        <| Option.defaultValue "" dto.CodigoMunicipio
        <| dto.CodigoCnae
        <| Option.defaultValue 0 dto.NaturezaOperacao
        <| Option.defaultValue 0 dto.RegimeEspecialTributacao
        <| Option.defaultValue false dto.OptanteSimplesNacional
        <| Option.defaultValue false dto.IncentivadorCultural
    | None -> fail ServicoIsRequired


let mapToTomadorDomain maybeTomadorDto =
    match maybeTomadorDto with
    | Some tomadorDto -> mapTomadorDomain tomadorDto
    | None -> fail TomadorIsRequired


let salvarNotaFiscal (request: CriarNotaFiscalRequest) =
    createNotaFiscalServico
    <| Guid.NewGuid()
    <| mapToTomadorDomain request.Tomador
    <| mapToServicoDomain request.Servico