module NotaFiscal.Domain.Tomador

open NotaFiscal.Domain.ApplicationErrors
open NotaFiscal.Domain.NotaFiscalPrimitives
open Endereco
open System

type Tomador =
    | PessoaFisica of TomadorPessoaFisica option
    | PessoaJuridica of TomadorPessoaJuridica
    | Estrangeiro of TomadorEstrangeiro

and TomadorPessoaFisica =
    { Id: Guid
      Cpf: CPF
      InscricaoMunicipal: MaybeStrMax15
      Endereco: Endereco option
      Contato: Contato option
      Nome: MaybeStrMax115 }

and TomadorPessoaJuridica =
    { Id: Guid
      Cnpj: CNPJ
      InscricaoMunicipal: MaybeStrMax15
      RazaoSocial: StrMax115
      Contato: Contato
      Endereco: Endereco }

and TomadorEstrangeiro =
    { Endereco: Endereco option
      Contato: Contato option
      RazaoSocial: MaybeStrMax115 }

and Contato = { Telefone: Telefone; Email: EmailAddress }

let createInscricaoMunicipalTomador inscricaoMunicipal mapErrors =
    StrMax15.createOptional inscricaoMunicipal
    |> mapFailuresR mapErrors

let createEmail email =
    let mapErrors strError =
        match strError with
        | StringError.Missing -> EmailIsRequired
        | MustNotBeLongerThan 80 -> EmailDoesntMatchPattern
        | DoesntMatchPattern _ -> EmailDoesntMatchPattern
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo email"

    EmailAddress.create email |> mapFailuresR mapErrors

let createTelefone telefone =
    let mapErrors strError =
        match strError with
        | StringError.Missing -> TelefoneIsRequired
        | StringError.MustBeBetween(8, 15) -> TelefoneDoesntMatchPattern
        | DoesntMatchPattern _ -> TelefoneDoesntMatchPattern
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo telefone"


    Telefone.create telefone |> mapFailuresR mapErrors

let createRazaoSocial razaoSocial =
    let mapErrors strError =
        match strError with
        | StringError.Missing -> RazaoSocialIsRequired
        | MustNotBeLongerThan 115 -> RazaoSocialMustNotBeMoreThan115Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo Razao Social"

    StrMax115.create razaoSocial |> mapFailuresR mapErrors

let createContato telefone email = { Telefone = telefone; Email = email }

let createContatoCompose telefone email =
    createContato <!> createTelefone telefone
    <*> createEmail email

let createContatoPessoaFisica telefone email =
    createContatoCompose telefone email
    |> mapFailuresR PessoaFisicaErrors.ContatoInvalido

let createContatoPessoaJuridica telefone email =
    createContatoCompose telefone email
    |> mapFailuresR ContatoInvalido

let createCnpjTomador cnpj =
    let mapErrors strError =
        match strError with
        | StringError.Missing -> CNPJIsRequired
        | DoesntMatchPattern _ -> CNPJDoesntMatchPattern
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo CNPJ"

    CNPJ.create cnpj |> mapFailuresR mapErrors

let createCpf cpf =
    let mapErrors strError =
        match strError with
        | StringError.Missing -> CPFIsRequired
        | DoesntMatchPattern _ -> CPFDoesntMatchPattern
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo CPF"

    CPF.create cpf |> mapFailuresR mapErrors

let createInscricaoMunicipalTomadorFisico inscricaoMunicipal =
    let mapErrors strError =
        match strError with
        | MustNotBeLongerThan 15 ->
            PessoaFisicaErrors.InscricaoMunicipalMustNotBeMoreThan15Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo Inscricao Municipal"

    createInscricaoMunicipalTomador inscricaoMunicipal mapErrors

let createInscricaoMunicipalTomadorJuridico inscricaoMunicipal =
    let mapErrors strError =
        match strError with
        | MustNotBeLongerThan 15 -> InscricaoMunicipalMustNotBeMoreThan15Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo Inscricao Municipal"

    createInscricaoMunicipalTomador inscricaoMunicipal mapErrors

let createEnderecoPessoaFisica
    id
    rua
    numero
    complemento
    bairro
    codigoMunicipio
    cep
    =
    createEndereco id rua numero complemento bairro codigoMunicipio cep
    |> mapFailuresR PessoaFisicaErrors.EnderecoInvalido

let createEnderecoPessoaJuridica
    id
    rua
    numero
    complemento
    bairro
    codigoMunicipio
    cep
    =
    createEndereco id rua numero complemento bairro codigoMunicipio cep
    |> mapFailuresR EnderecoInvalido


let toNotaFiscalInvalida status =
    mapFailuresR (status >> TomadorInvalido)

let createTomadorPessoaJuridica
    id
    cnpjResult
    inscricaoMunicipalResult
    razaoSocialResult
    enderecoResult
    contatoResult
    =
    let createTomadorPessoaJuridica'
        id
        cnpj
        inscricaoMunicipal
        razaoSocial
        contato
        endereco
        =
        { Id = id
          Cnpj = cnpj
          InscricaoMunicipal = inscricaoMunicipal
          RazaoSocial = razaoSocial
          Contato = contato
          Endereco = endereco }
        |> PessoaJuridica

    createTomadorPessoaJuridica' id <!> cnpjResult
    <*> inscricaoMunicipalResult
    <*> razaoSocialResult
    <*> contatoResult
    <*> enderecoResult
    |> toNotaFiscalInvalida PessoaJuridicaInvalida

let createTomadorPessoaFisica
    id
    cpfResult
    inscricaoMunicipalResult
    enderecoResult
    contatoResult
    =
    let createTomadorPessoaFisica'
        id
        cpf
        inscricaoMunicipal
        endereco
        contato
        nome
        =
        { Id = id
          Cpf = cpf
          InscricaoMunicipal = inscricaoMunicipal
          Endereco = endereco
          Contato = contato
          Nome = nome }
        |> Some
        |> PessoaFisica

    createTomadorPessoaFisica' id <!> cpfResult
    <*> inscricaoMunicipalResult
    <*> enderecoResult
    <*> contatoResult
    |> toNotaFiscalInvalida PessoaFisicaInvalida

let tomadorEstrangeiro endereco contato razaoSocial =
    Estrangeiro
        { Endereco = endereco; Contato = contato; RazaoSocial = razaoSocial }