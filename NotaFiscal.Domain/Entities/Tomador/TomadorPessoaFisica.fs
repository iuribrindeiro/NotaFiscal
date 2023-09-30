module NotaFiscal.Domain.Entities.TomadorPessoaFisica

open NotaFiscal.Domain
open NotaFiscal.Domain.Entities.Endereco
open NotaFiscal.Domain.Entities.TomadorShared

type TomadorPessoaFisica =
    private
        { Cpf: CPF
          InscricaoMunicipal: MaybeStrMax15
          Endereco: Endereco option
          Contato: Contato option
          Nome: MaybeStrMax115 }

[<AutoOpen>]
module TomadorPessoaFisicaUtils =
    let createContatoPessoaFisica telefone email =
        let mapFailuresContatoPessoaFisica =
            PessoaFisicaErrors.ContatoInvalido
            >> PessoaFisicaInvalida
            >> TomadorInvalido

        createContato telefone email
        |> mapFailuresR mapFailuresContatoPessoaFisica


    let createEnderecoPessoaFisica
        rua
        numero
        complemento
        bairro
        codigoMunicipio
        cep
        =
        let mapErrors =
            PessoaFisicaErrors.EnderecoInvalido
            >> PessoaFisicaInvalida
            >> TomadorInvalido

        createEndereco rua numero complemento bairro codigoMunicipio cep
        |> mapFailuresR mapErrors


    let createTomadorPessoaFisica cpf inscricaoMunicipal nome =
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
                    |> failwithf
                        "%s não esperado para o campo Inscricao Municipal"

            createInscricaoMunicipalTomador inscricaoMunicipal mapErrors

        let createNomePessoaFisica nome =
            let mapErrors strError =
                match strError with
                | MustNotBeLongerThan 115 ->
                    PessoaFisicaErrors.NomeMustNotBeMoreThan115Chars
                | x ->
                    x.GetType().Name
                    |> failwithf
                        "%s não esperado para o campo Nome do tomador de pessoa fisica"

            StrMax115.createOptional nome |> mapFailuresR mapErrors


        let mapErrors =
            PessoaFisicaInvalida >> TomadorInvalido |> mapFailuresR


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

        createTomadorPessoaFisica' <!> createCpf cpf
        <*> createInscricaoMunicipalTomadorFisico inscricaoMunicipal
        <*> createNomePessoaFisica nome
        |> mapErrors
