module NotaFiscal.Domain.Tomador

open NotaFiscal.Domain.DomainEvents
open NotaFiscal.Domain.NotaFiscalPrimitives
open Endereco
open System

type Tomador =
    | PessoaFisica of TomadorPessoaFisica option
    | PessoaJuridica of TomadorPessoaJuridica
    | Estrangeiro of TomadorEstrangeiro

and TomadorPessoaFisica =
    { Cpf: CPF
      InscricaoMunicipal: MaybeStrMax15
      Endereco: Endereco option
      Contato: Contato option
      Nome: MaybeStrMax115 }

and TomadorPessoaJuridica =
    { Cnpj: CNPJ
      InscricaoMunicipal: MaybeStrMax15
      RazaoSocial: StrMax115
      Contato: Contato
      Endereco: Endereco }

and TomadorEstrangeiro =
    { Endereco: Endereco option
      Contato: Contato option
      Nome: MaybeStrMax115 }

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
        | MustNotBeLongerThan 115 -> PessoaJuridicaErrors.RazaoSocialMustNotBeMoreThan115Chars
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
    |> mapFailuresR (PessoaFisicaErrors.ContatoInvalido >> PessoaFisicaInvalida >> TomadorInvalido)

let createContatoPessoaJuridica telefone email =
    createContatoCompose telefone email
    |> mapFailuresR (PessoaJuridicaErrors.ContatoInvalido >> PessoaJuridicaInvalida >> TomadorInvalido)
    
let createContatoTomadorEstrangeiro telefone email =
    createContatoCompose telefone email
    |> mapFailuresR (TomadorEstrangeiroErrors.ContatoInvalido >> TomadorEstrangeiroInvalido >> TomadorInvalido)

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

let createNomePessoaFisica nome =
    let mapErrors strError =
        match strError with
        | MustNotBeLongerThan 115 ->
            PessoaFisicaErrors.NomeMustNotBeMoreThan115Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo Nome do tomador de pessoa fisica"
    
    StrMax115.createOptional nome |> mapFailuresR mapErrors

let createInscricaoMunicipalTomadorJuridico inscricaoMunicipal =
    let mapErrors strError =
        match strError with
        | MustNotBeLongerThan 15 -> InscricaoMunicipalMustNotBeMoreThan15Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo Inscricao Municipal"

    createInscricaoMunicipalTomador inscricaoMunicipal mapErrors

let createEnderecoPessoaFisica
    rua
    numero
    complemento
    bairro
    codigoMunicipio
    cep
    =
    createEndereco rua numero complemento bairro codigoMunicipio cep
    |> mapFailuresR (PessoaFisicaErrors.EnderecoInvalido >> PessoaFisicaInvalida >> TomadorInvalido)

let createEnderecoPessoaJuridica
    rua
    numero
    complemento
    bairro
    codigoMunicipio
    cep
    =
    createEndereco rua numero complemento bairro codigoMunicipio cep
    |> mapFailuresR (PessoaJuridicaErrors.EnderecoInvalido >> PessoaJuridicaInvalida >> TomadorInvalido)

let createEnderecoTomadorEstrangeiro
    rua
    numero
    complemento
    bairro
    codigoMunicipio
    cep
    =
    createEndereco rua numero complemento bairro codigoMunicipio cep
    |> mapFailuresR (TomadorEstrangeiroErrors.EnderecoInvalido >> TomadorEstrangeiroInvalido >> TomadorInvalido)

let toNotaFiscalInvalida status =
    mapFailuresR (status >> TomadorInvalido)

let createTomadorPessoaJuridica
    cnpj
    incricaoMunicipal
    razaoSocial
    =
    let createTomadorPessoaJuridica'
        cnpj
        inscricaoMunicipal
        razaoSocial
        contato
        endereco
        =
        { Cnpj = cnpj
          InscricaoMunicipal = inscricaoMunicipal
          RazaoSocial = razaoSocial
          Contato = contato
          Endereco = endereco }
        |> PessoaJuridica

    createTomadorPessoaJuridica'
        <!> createCnpjTomador cnpj
        <*> createInscricaoMunicipalTomadorJuridico incricaoMunicipal
        <*> createRazaoSocial razaoSocial
    |> toNotaFiscalInvalida PessoaJuridicaInvalida

let createNomeTomadorEstrangeiro nome =
    let mapErrors strError =
        match strError with
        | MustNotBeLongerThan 115 -> TomadorEstrangeiroErrors.NomeMustNotBeMoreThan115Chars
        | x ->
            x.GetType().Name
            |> failwithf "%s não esperado para o campo Razao Social"
    
    StrMax115.createOptional nome |> mapFailuresR mapErrors

let createTomadorEstrangeiro nome =
    let createTomadorEstrangeiro' nome endereco contato =
        { Nome = nome; Contato = contato; Endereco = endereco } |> Estrangeiro
        
    createTomadorEstrangeiro'
        <!> createNomeTomadorEstrangeiro nome
    |> toNotaFiscalInvalida TomadorEstrangeiroInvalido

let createTomadorPessoaFisica
    cpf
    inscricaoMunicipal
    nome
    =
    let createTomadorPessoaFisica'
        cpf
        inscricaoMunicipal
        nome
        endereco
        contato
        =
        { Cpf = cpf
          InscricaoMunicipal = inscricaoMunicipal
          Endereco = endereco
          Contato = contato
          Nome = nome }
        |> Some
        |> PessoaFisica

    createTomadorPessoaFisica'
        <!> createCpf cpf
        <*> createInscricaoMunicipalTomadorFisico inscricaoMunicipal
        <*> createNomePessoaFisica nome
    
    |> toNotaFiscalInvalida PessoaFisicaInvalida

let tomadorEstrangeiro endereco contato razaoSocial =
    Estrangeiro
        { Endereco = endereco; Contato = contato; Nome = razaoSocial }