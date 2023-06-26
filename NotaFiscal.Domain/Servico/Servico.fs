module NotaFiscal.Domain.Servico

open NotaFiscal.Domain.NotaFiscalPrimitives
open NotaFiscal.Domain.DomainEvents
open NotaFiscal.Domain.ValoresServico


type Servico =
    { Valores: Valores
      ItemListaServico: StrMax7
      CodigoTributacaoMunicipio: MaybeStrMax20
      Discriminacao: StrMax2000
      CodigoMunicipioPrestacao: StrMax7
      CodigoCnae: MaybeStrMax7
      NaturezaOperacao: NaturezaOperacao
      RegimeEspecialTributacao: RegimeEspecialTributacao
      OptanteSimplesNacional: bool
      IncentivadorCultural: bool }
    
let createItemListaServico itemListaServico =
    let mapErrors strError =
        match strError with
        | StringError.Missing -> ItemListaServicoIsRequired
        | StringError.MustNotBeLongerThan 7 -> ItemListaServicoMustNotBeMoreThan7Chars
        | x -> x.GetType().Name |> failwithf "%s não esperado para o campo ItemListaServico"
    
    StrMax7.create itemListaServico
    |> mapFailuresR mapErrors

let createCodigoTributacaoMunicipio codigoTributacaoMunicipio =
    let mapErrors strError =
        match strError with
        | StringError.MustNotBeLongerThan 20 -> CodigoTributacaoMunicipioMustNotBeMoreThan20Chars
        | x -> x.GetType().Name |> failwithf "%s não esperado para o campo CodigoTributacaoMunicipio"
    
    StrMax20.createOptional codigoTributacaoMunicipio
    |> mapFailuresR mapErrors

let createDiscriminacao discriminacao =
    let mapErrors strError =
        match strError with
        | StringError.Missing -> DiscriminacaoIsRequired
        | StringError.MustNotBeLongerThan 2000 -> DiscriminacaoMustNotBeMoreThan2000Chars
        | x -> x.GetType().Name |> failwithf "%s não esperado para o campo Discriminacao"
    
    StrMax2000.create discriminacao
    |> mapFailuresR mapErrors

let createCodigoMunicipio codigoMunicipio =
    let mapErrors strError =
        match strError with
        | StringError.Missing -> CodigoMunicipioIsRequired
        | StringError.MustNotBeLongerThan 7 -> CodigoMunicipioMustNotBeMoreThan7Chars
        | x -> x.GetType().Name |> failwithf "%s não esperado para o campo CodigoMunicipio"
    
    StrMax7.create codigoMunicipio
    |> mapFailuresR mapErrors

let createCodigoCnae codigoCnae =
    let mapErrors strError =
        match strError with
        | StringError.MustNotBeLongerThan 7 -> CodigoCnaeMustNotBeMoreThan7Chars
        | x -> x.GetType().Name |> failwithf "%s não esperado para o campo CodigoCnae"
    
    StrMax7.createOptional codigoCnae
    |> mapFailuresR mapErrors

let createNaturezaOperacao naturezaOperacao =
    NaturezaOperacao.create naturezaOperacao |> mapFailuresR (fun _ -> NaturezaOperacaoInvalida)

let createRegimeEspecialTributacao regimeEspecialTributacao =
    RegimeEspecialTributacao.create regimeEspecialTributacao |> mapFailuresR (fun _ -> RegimeEspecialTributacaoInvalido)


let createServico
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
    let createServico'
        itemListaServico
        codigoTributacaoMunicipio
        discriminacao
        codigoMunicipio
        codigoCnae
        naturezaOperacao
        regimeEspecialTributacao
        optanteSimplesNacional
        incentivadorCultural
        valores
        : Servico
        =
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

    createServico'
        <!> createItemListaServico itemListaServico
        <*> createCodigoTributacaoMunicipio codigoTributacaoMunicipio
        <*> createDiscriminacao discriminacao
        <*> createCodigoMunicipio codigoMunicipio
        <*> createCodigoCnae codigoCnae
        <*> createNaturezaOperacao naturezaOperacao
        <*> createRegimeEspecialTributacao regimeEspecialTributacao
        <*> succeed optanteSimplesNacional
        <*> succeed incentivadorCultural
    |> mapFailuresR ServicoInvalido