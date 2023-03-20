module NotaFiscal.Data.Mappers.TomadorMapper

open NotaFiscal.Domain.Creators.TomadorCreator
open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.ValidationErrors
open NotaFiscal.Data.DbAutoGen

let stringOrEmpty optionalValue = Option.defaultValue "" optionalValue


let createEnderecoFromDb createEndereco (endereco: dbo.Endereco) =
    createEndereco
    <| endereco.Rua
    <| endereco.Numero
    <| endereco.Complemento
    <| endereco.Bairro
    <| endereco.CodigoMunicipio
    <| endereco.Uf
    <| endereco.Cep

let createContatoFromDb createContato telefone email = createContato telefone email


module TomadorPessoaFisicaDb =

    let createEnderecoPessoaFisicaFromDb (enderecoDb: dbo.Endereco option) =
        match enderecoDb with
        | Some e ->
            let enderecoResult = createEnderecoFromDb createEnderecoPessoaFisica e
            Some <!> enderecoResult
        | None -> succeed None

    let createContatoPessoaFisicaFromDb (contatoDb: dbo.Contato option) =
        match contatoDb with
        | Some c ->
            let contatoResult = createContatoFromDb createContatoPessoaFisica c.Telefone c.Email

            Some <!> contatoResult
        | None -> succeed None

    let craeteTomadorPessoaFisicaFromDb cpf inscricaoMunicipal enderecoPessoaFisica contatoPessoaFisica =
        createTomadorPessoaFisica
        <| createCpfPessoaFisica cpf
        <| createInscricaoMunicipalTomadorFisico inscricaoMunicipal
        <| enderecoPessoaFisica
        <| contatoPessoaFisica

module TomadorPessoaJuridicaDb =

    let createEnderecoPessoaJuridicaFromDb (enderecoDb: dbo.Endereco option) =
        match enderecoDb with
        | Some e -> createEnderecoFromDb createEnderecoPessoaJuridica e
        | None -> fail FailConvertEnderecoFromDb

    let createContatoPessoaJuridicaFromDb (contatoDb: dbo.Contato option) =
        match contatoDb with
        | Some c -> createContatoFromDb createContatoPessoaJuridica c.Telefone c.Email
        | None -> fail FailConvertContatoFromDb

    let createTomadorPessoaJuridicaFromDb
        cnpj
        inscricaoMunicipal
        (razaoSocial: string option)
        enderecoPessoaJuridica
        contatoPessoaJuridica
        =
        createTomadorPessoaJuridica
        <| createCnpjTomador cnpj
        <| createInscricaoMunicipalTomadorJuridico inscricaoMunicipal
        <| createRazaoSocial (razaoSocial |> Option.defaultValue "")
        <| contatoPessoaJuridica
        <| enderecoPessoaJuridica

let createTomadorFromDb
    (tomadorDb: dbo.Tomador option)
    (enderecoDb: dbo.Endereco option)
    (contatoDb: dbo.Contato option)
    (tomadorDiscriminator)
    =
    match (tomadorDiscriminator, tomadorDb) with
    | nameof (PessoaFisica), Some tomador ->
        let enderecoPessoaFisica =
            TomadorPessoaFisicaDb.createEnderecoPessoaFisicaFromDb enderecoDb

        let contatoPessoaFisica =
            TomadorPessoaFisicaDb.createContatoPessoaFisicaFromDb contatoDb


        TomadorPessoaFisicaDb.craeteTomadorPessoaFisicaFromDb
            tomador.CpfCnpj
            tomador.InscricaoMunicipal
            enderecoPessoaFisica
            contatoPessoaFisica
    | nameof (PessoaJuridica), Some tomador ->
        let enderecoPessoaJuridica =
            TomadorPessoaJuridicaDb.createEnderecoPessoaJuridicaFromDb enderecoDb

        let contatoPessoaJuridica =
            TomadorPessoaJuridicaDb.createContatoPessoaJuridicaFromDb contatoDb


        let tomadorPessoaJuridica =
            TomadorPessoaJuridicaDb.createTomadorPessoaJuridicaFromDb
                tomador.CpfCnpj
                tomador.InscricaoMunicipal
                tomador.RazaoSocial
                enderecoPessoaJuridica
                contatoPessoaJuridica

        tomadorPessoaJuridica
    | nameof (Estrangeiro), None -> succeed createTomadorEstrangeiro
    | nameof (PessoaFisica), None -> PessoaFisica None |> succeed
    | _ -> fail FailToConvertTomadorFromDbError
