module NotaFiscal.Domain.Creators.ServicoCreator

open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.NotaFiscalPrimitives
open NotaFiscal.Domain.ValidationErrors

let private createServico'
    valores
    itemListaServico
    codigoTributacaoMunicipio
    discriminacao
    codigoMunicipio
    codigoCnae
    naturezaOperacao
    regimeEspecialTributacao
    optanteSimplesNacional
    incentivadorCultural
    : Servico =
    { Valores = valores
      ItemListaServico = itemListaServico
      CodigoTributacaoMunicipio = codigoTributacaoMunicipio
      Discriminacao = discriminacao
      CodigoMunicipioPrestacao = codigoMunicipio
      CodigoCnae = codigoCnae
      NaturezaOperacao = naturezaOperacao
      RegimeEspecialTributacao = regimeEspecialTributacao
      OptanteSimplesNacional = optanteSimplesNacional
      IncentivadorCultural = incentivadorCultural }

let private createValoresServico'
    servicos
    deducoes
    pis
    cofins
    inss
    ir
    csll
    outrasRetencoes
    iss
    aliquota
    descontoIncondicionado
    descontoCondicionado
    : Valores =
    { Servicos = servicos
      Deducoes = deducoes
      Pis = pis
      Cofins = cofins
      Inss = inss
      Ir = ir
      Csll = csll
      OutrasRetencoes = outrasRetencoes
      Iss = iss
      Aliquota = aliquota
      DescontoIncondicionado = descontoIncondicionado
      DescontoCondicionado = descontoCondicionado }


let createItemListaServico itemListaServico =
    StringMax7.create itemListaServico |> mapFailures ItemListaServicoInvalido

let createCodigoTributacaoMunicipio codigoTributacaoMunicipio =
    StringMax20.createOptional codigoTributacaoMunicipio
    |> mapFailures CodigoTributacaoMunicipioInvalido

let createDiscriminacao discriminacao =
    StringMax2000.create discriminacao |> mapFailures DiscriminacaoInvalida

let createCodigoMunicipio codigoMunicipio =
    CodigoMunicipio <!> StringMax7.create codigoMunicipio
    |> mapFailures ServicoCodigoMunicipioInvalido

let createCodigoCnae codigoCnae =
    StringMax7.createOptional codigoCnae |> mapFailures CodigoCnaeInvalido

let createNaturezaOperacao naturezaOperacao =
    NaturezaOperacao.create naturezaOperacao

let createRegimeEspecialTributacao regimeEspecialTributacao =
    RegimeEspecialTributacao.create regimeEspecialTributacao

let createValorServicos valor =
    ValorDinheiro.create valor |> mapFailures ValorServicoInvalido

let createValorDeducoes valor =
    ValorDinheiro.createOptional valor |> mapFailures ValorDeducoesInvalido

let createValorPis valor =
    ValorDinheiro.createOptional valor |> mapFailures ValorPisInvalido

let createValorCofins valor =
    ValorDinheiro.createOptional valor |> mapFailures ValorCofinsInvalido

let createValorInss valor =
    ValorDinheiro.createOptional valor |> mapFailures ValorInssInvalido

let createValorIr valor =
    ValorDinheiro.createOptional valor |> mapFailures ValorIrInvalido

let createValorCsll valor =
    ValorDinheiro.createOptional valor |> mapFailures ValorCsllInvalido

let createValorIssRetido valor =
    Retido <!> ValorDinheiro.create valor
    |> mapFailures (IssRetidoInvalido >> IssInvalido)

let createValorIssNaoRetido valor =
    NaoRetido <!> ValorDinheiro.create valor
    |> mapFailures (IssNaoRetidoInvalido >> IssInvalido)

let createValorOutrasRetencoes valor =
    ValorDinheiro.createOptional valor |> mapFailures OutrasRetencoesInvalido

let createValorDescontoIncondicionado valor =
    ValorDinheiro.createOptional valor |> mapFailures DescontoIncondicionadoInvalido

let createValorDescontoCondicionado valor =
    ValorDinheiro.createOptional valor |> mapFailures DescontoCondicionadoInvalido

let createAliquota valor =
    Percentage.createOptional valor |> mapFailures AliquotaInvalida


let createServico
    valoresResult
    itemListaServico
    codigoTributacaoMunicipio
    discriminacao
    codigoMunicipio
    codigoCnae
    naturezaOperacao
    regimeEspecialTributacao
    optanteSimplesNacional
    incentivadorCultural
    =
    let servicoResult =
        createServico' <!> (valoresResult |> mapFailures ValoresInvalidos)
        <*> createItemListaServico itemListaServico
        <*> createCodigoTributacaoMunicipio codigoTributacaoMunicipio
        <*> createDiscriminacao discriminacao
        <*> createCodigoMunicipio codigoMunicipio
        <*> createCodigoCnae codigoCnae
        <*> createNaturezaOperacao naturezaOperacao
        <*> createRegimeEspecialTributacao regimeEspecialTributacao

    match servicoResult with
    | Success(servico, _) ->
        let servico = servico optanteSimplesNacional incentivadorCultural
        servico |> succeed
    | Failure errors -> Failure errors


let createValoresServico
    valorServicos
    deducoes
    pis
    cofins
    inss
    ir
    csll
    iss
    issDiscriminator
    outrasRetencoes
    descontoCondicionado
    descontoIncondicionado
    aliquota
    =
    let createValorIss valorIss issType =
        match issType with
        | nameof (Retido) -> createValorIssRetido valorIss
        | nameof (NaoRetido) -> createValorIssNaoRetido valorIss
        | tipo -> TipoIssInvalido tipo |> IssInvalido |> fail

    createValoresServico' <!> createValorServicos valorServicos
    <*> createValorDeducoes deducoes
    <*> createValorPis pis
    <*> createValorCofins cofins
    <*> createValorInss inss
    <*> createValorIr ir
    <*> createValorCsll csll
    <*> createValorOutrasRetencoes outrasRetencoes
    <*> createValorIss iss issDiscriminator
    <*> createAliquota aliquota
    <*> createValorDescontoIncondicionado descontoIncondicionado
    <*> createValorDescontoCondicionado descontoCondicionado
