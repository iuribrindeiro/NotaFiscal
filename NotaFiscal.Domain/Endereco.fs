module NotaFiscal.Domain.Endereco

open NotaFiscal.Domain.ApplicationErrors
open NotaFiscal.Domain.NotaFiscalPrimitives
open System


type Endereco =
    { Rua: StrMax120
      Numero: StrMax60
      Complemento: MaybeStrMax60
      Bairro: StrMax60
      CodigoMunicipio: StrMax7
      Cep: StrOf8 }

let createRua rua =
    let mapRuaErrors strError =
        match strError with
        | StringError.Missing -> EnderecoError.RuaIsRequired
        | StringError.MustNotBeLongerThan 120 ->
            EnderecoError.RuaMustNotBeMoreThan120Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo rua"

    StrMax120.create rua |> mapFailuresR mapRuaErrors

let createNumero numero =
    let mapNumeroErrors strError =
        match strError with
        | StringError.Missing -> EnderecoError.NumeroIsRequired
        | StringError.MustNotBeLongerThan 60 ->
            EnderecoError.NumeroMustNotBeMoreThan60Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo numero"

    StrMax60.create numero |> mapFailuresR mapNumeroErrors

let createComplemento complemento =
    let mapComplementoErrors strError =
        match strError with
        | StringError.MustNotBeLongerThan 60 ->
            EnderecoError.ComplementoMustNotBeMoreThan60Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo complemento"

    StrMax60.createOptional complemento
    |> mapFailuresR mapComplementoErrors

let createBairro bairro =
    let mapBairroErrors strError =
        match strError with
        | StringError.Missing -> EnderecoError.BairroIsRequired
        | StringError.MustNotBeLongerThan 60 ->
            EnderecoError.BairroMustNotBeMoreThan60Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo bairro"

    StrMax60.create bairro |> mapFailuresR mapBairroErrors

let createCodigoMunicipio codigoMunicipio =
    let mapCodigoMunicipioErrors strErrors =
        match strErrors with
        | StringError.Missing -> EnderecoError.CodigoMunicipioIsRequired
        | StringError.MustNotBeLongerThan 7 ->
            EnderecoError.CodigoMunicipioMustNotBeMoreThan7Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo codigoMunicipio"

    StrMax7.create codigoMunicipio
    |> mapFailuresR mapCodigoMunicipioErrors

let createUF uf =
    let mapUfErrors strUfErrors =
        match strUfErrors with
        | StringError.Missing -> EnderecoError.UfIsRequired
        | StringError.MustHaveLen 2 -> EnderecoError.UfMustBe2Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo uf"

    StrOf2.create uf |> mapFailuresR mapUfErrors

let createCep cep =
    let mapCepErrors strCepErrors =
        match strCepErrors with
        | StringError.Missing -> EnderecoError.CepIsRequired
        | StringError.MustHaveLen 8 -> EnderecoError.CepMustBe8Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo cep"

    StrOf8.create cep |> mapFailuresR mapCepErrors

let createEndereco
    (id: Guid)
    (rua: string)
    (numero: string)
    (complemento: string option)
    (bairro: string)
    (codigoMunicipio: string)
    (cep: string)
    : OperationResult<Endereco, EnderecoError>
    =
    let createEndereco' rua numero complemento bairro codigoMunicipio cep =
        { Rua = rua
          Numero = numero
          Complemento = complemento
          Bairro = bairro
          CodigoMunicipio = codigoMunicipio
          Cep = cep }

    createEndereco' <!> createRua rua
    <*> createNumero numero
    <*> createComplemento complemento
    <*> createBairro bairro
    <*> createCodigoMunicipio codigoMunicipio
    <*> createCep cep