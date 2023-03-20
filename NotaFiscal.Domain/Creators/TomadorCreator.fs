module NotaFiscal.Domain.Creators.TomadorCreator

open NotaFiscal.Domain.NotaFiscalPrimitives
open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.ValidationErrors
open EnderecoCreator

let createInscricaoMunicipalTomador inscricaoMunicipal mapErrors =
    inscricaoMunicipal |> StringMax15.createOptional |> mapFailures mapErrors


let createEmail email =
    EmailAddress.create email |> mapFailures EmailInvalido

let createTelefone telefone =
    Telefone.create telefone |> mapFailures TelefoneInvalido

let createRazaoSocial razaoSocial =
    StringMax115.create razaoSocial |> mapFailures RazaoSocialInvalida

let createTomador pessoaFisica pessoaJuridica estrangeiro =
    match pessoaFisica, pessoaJuridica, estrangeiro with
    | Some pf, None, None -> PessoaFisica pf |> succeed
    | None, Some pj, None -> PessoaJuridica pj |> succeed
    | None, None, Some e -> Estrangeiro e |> succeed
    | None, None, None -> PessoaFisica None |> succeed
    | _ -> fail TomadorDeveSerFisicoJuricoOuEstrangeiro


let createContato telefone email = { Telefone = telefone; Email = email }

let createContatoCompose telefone email =
    createContato <!> createTelefone telefone <*> createEmail email

let createContatoPessoaFisica telefone email =
    createContatoCompose telefone email |> mapFailures PessoaFisicaContatoInvalido

let createContatoPessoaJuridica telefone email =
    createContatoCompose telefone email |> mapFailures PessoaJuridicaContatoInvalido

let createCnpjPrestador cnpj = createCnpj cnpj PrestadorCNPJInvalido

let createCnpjTomador cnpj =
    createCnpj cnpj PessoaJuridicaCNPJInvalido

let createCpfPessoaFisica cpf =
    CPF.create cpf |> mapFailures CPFInvalido

let createInscricaoMunicipalTomadorFisico inscricaoMunicipal =
    createInscricaoMunicipalTomador inscricaoMunicipal PessoaFisicaInscricaoMunicipalInvalida

let createInscricaoMunicipalTomadorJuridico inscricaoMunicipal =
    createInscricaoMunicipalTomador inscricaoMunicipal PessoaJuridicaInscricaoMunicipalInvalida

let createEnderecoPessoaFisica rua numero complemento bairro codigoMunicipio uf cep =
    createEnderecoCompose rua numero complemento bairro codigoMunicipio uf cep
    |> mapFailures PessoaFisicaEnderecoInvalido

let createEnderecoPessoaJuridica rua numero complemento bairro codigoMunicipio uf cep =
    createEnderecoCompose rua numero complemento bairro codigoMunicipio uf cep
    |> mapFailures PessoaJuridicaEnderecoInvalido


//Essas funcoes sao internas para evitar a criacao de um tomador sem um tipo especifico, PessoaJuridica ou PessoaFisica, por clients usando o modulo
let internal createTomadorPessoaJuridica' cnpj inscricaoMunicipal razaoSocial contato endereco =
    { Cnpj = cnpj
      InscricaoMunicipal = inscricaoMunicipal
      RazaoSocial = razaoSocial
      Contato = contato
      Endereco = endereco }
    |> PessoaJuridica

let internal createTomadorPessoaFisica' cpf inscricaoMunicipal endereco contato =
    { Cpf = cpf
      InscricaoMunicipal = inscricaoMunicipal
      Endereco = endereco
      Contato = contato }
    |> Some
    |> PessoaFisica

let createTomadorPessoaJuridica cnpjResult inscricaoMunicipalResult razaoSocialResult contatoResult enderecoResult =
    createTomadorPessoaJuridica' <!> cnpjResult
    <*> inscricaoMunicipalResult
    <*> razaoSocialResult
    <*> contatoResult
    <*> enderecoResult
    |> mapFailures PessoaJuridicaInvalida

let createTomadorPessoaFisica cpfResult inscricaoMunicipalResult enderecoResult contatoResult =
    createTomadorPessoaFisica' <!> cpfResult
    <*> inscricaoMunicipalResult
    <*> enderecoResult
    <*> contatoResult
    |> mapFailures PessoaFisicaInvalida

let createEmptyTomadorPessoaFisica = PessoaFisica None

let createTomadorEstrangeiro = Estrangeiro TomadorEstrangeiro
