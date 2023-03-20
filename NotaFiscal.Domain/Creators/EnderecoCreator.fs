module internal NotaFiscal.Domain.Creators.EnderecoCreator

open NotaFiscal.Domain.NotaFiscalPrimitives
open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.ValidationErrors

let internal createEndereco rua numero complemento bairro codigoMunicipio uf cep =
    { Rua = rua
      Numero = numero
      Complemento = complemento
      Bairro = bairro
      CodigoMunicipio = codigoMunicipio
      UF = uf
      Cep = cep }

let createRua rua =
    StringMax120.create rua |> mapFailures RuaInvalida

let createNumero numero =
    StringMax60.create numero |> mapFailures NumeroInvalido

let createComplemento complemento =
    StringMax60.createOptional complemento |> mapFailures ComplementoInvalido

let createBairro bairro =
    StringMax60.create bairro |> mapFailures BairroInvalido

let createCodigoMunicipio codigoMunicipio =
    CodigoMunicipio <!> StringMax7.create codigoMunicipio
    |> mapFailures CodigoMunicipioInvalido

let createUF uf = UF.create uf |> mapFailures UFInvalida

let createCep cep =
    String8.create cep |> mapFailures CepInvalido

let createEnderecoCompose rua numero complemento bairro codigoMunicipio uf cep =
    createEndereco <!> createRua rua
    <*> createNumero numero
    <*> createComplemento complemento
    <*> createBairro bairro
    <*> createCodigoMunicipio codigoMunicipio
    <*> createUF uf
    <*> createCep cep
