module NotaFiscal.Domain.Entities.TomadorEstrangeiro

open NotaFiscal.Domain
open NotaFiscal.Domain.Entities.Endereco
open NotaFiscal.Domain.Entities.TomadorShared

type TomadorEstrangeiro =
    private
        { Endereco: Endereco option
          Contato: Contato option
          Nome: MaybeStrMax115 }


module TomadorEstrangeiroUtils =

    let createContatoTomadorEstrangeiro telefone email =
        let mapErrors =
            TomadorEstrangeiroErrors.ContatoInvalido
            >> TomadorEstrangeiroInvalido
            >> TomadorInvalido


        createContato telefone email |> mapFailuresR mapErrors


    let createEnderecoTomadorEstrangeiro
        rua
        numero
        complemento
        bairro
        codigoMunicipio
        cep
        =
        createEndereco rua numero complemento bairro codigoMunicipio cep
        |> mapFailuresR (
            TomadorEstrangeiroErrors.EnderecoInvalido
            >> TomadorEstrangeiroInvalido
            >> TomadorInvalido
        )

    let createTomadorEstrangeiro nome =
        let createNomeTomadorEstrangeiro nome =
            let mapErrors =
                function
                | MustNotBeLongerThan 115 ->
                    TomadorEstrangeiroErrors.NomeMustNotBeMoreThan115Chars
                | x ->
                    x.GetType().Name
                    |> failwithf "%s não esperado para o campo Razao Social"

            StrMax115.createOptional nome |> mapFailuresR mapErrors

        let createTomadorEstrangeiro' nome endereco contato =
            { Nome = nome; Contato = contato; Endereco = endereco }

        let mapErrors =
            TomadorEstrangeiroInvalido >> TomadorInvalido

        createTomadorEstrangeiro'
        <!> createNomeTomadorEstrangeiro nome
        |> mapFailuresR mapErrors
