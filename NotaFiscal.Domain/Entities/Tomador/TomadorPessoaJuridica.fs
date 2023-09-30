module NotaFiscal.Domain.Entities.TomadorPessoaJuridica

open NotaFiscal.Domain
open NotaFiscal.Domain.Entities.Endereco
open NotaFiscal.Domain.Entities.TomadorShared

type TomadorPessoaJuridica =
    private
        { Cnpj: CNPJ
          InscricaoMunicipal: MaybeStrMax15
          RazaoSocial: StrMax115
          Contato: Contato
          Endereco: Endereco }

[<AutoOpen>]
module TomadorPessoaJuridicaUtils =
    let createEnderecoPessoaJuridica
        rua
        numero
        complemento
        bairro
        codigoMunicipio
        cep
        =
        let mapToTomadorInvalido =
            PessoaJuridicaErrors.EnderecoInvalido
            >> PessoaJuridicaInvalida
            >> TomadorInvalido

        createEndereco rua numero complemento bairro codigoMunicipio cep
        |> mapFailuresR mapToTomadorInvalido

    let createContatoPessoaJuridica telefone email =
        let mapToTomadorInvalido =
            PessoaJuridicaErrors.ContatoInvalido
            >> PessoaJuridicaInvalida
            >> TomadorInvalido

        createContato telefone email
        |> mapFailuresR mapToTomadorInvalido


    let createTomadorPessoaJuridica cnpj incricaoMunicipal razaoSocial =
        let createRazaoSocial razaoSocial =
            let mapErrors =
                function
                | StringError.Missing -> RazaoSocialIsRequired
                | MustNotBeLongerThan 115 ->
                    PessoaJuridicaErrors.RazaoSocialMustNotBeMoreThan115Chars
                | x ->
                    x.GetType().Name
                    |> failwithf "%s não esperado para o campo Razao Social"

            StrMax115.create razaoSocial |> mapFailuresR mapErrors


        let createCnpjTomador cnpj =
            let mapErrors =
                function
                | StringError.Missing -> CNPJIsRequired
                | DoesntMatchPattern _ -> CNPJDoesntMatchPattern
                | x ->
                    x.GetType().Name
                    |> failwithf "%s não esperado para o campo CNPJ"

            CNPJ.create cnpj |> mapFailuresR mapErrors

        let createInscricaoMunicipalTomadorJuridico inscricaoMunicipal =
            let mapErrors =
                function
                | MustNotBeLongerThan 15 ->
                    InscricaoMunicipalMustNotBeMoreThan15Chars
                | x ->
                    x.GetType().Name
                    |> failwithf
                        "%s não esperado para o campo Inscricao Municipal"

            createInscricaoMunicipalTomador inscricaoMunicipal mapErrors

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

        let mapErrors =
            PessoaJuridicaInvalida >> TomadorInvalido |> mapFailuresR

        createTomadorPessoaJuridica' <!> createCnpjTomador cnpj
        <*> createInscricaoMunicipalTomadorJuridico incricaoMunicipal
        <*> createRazaoSocial razaoSocial
        |> mapErrors
