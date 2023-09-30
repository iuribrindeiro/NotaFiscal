module NotaFiscal.Domain.Entities.TomadorShared

open NotaFiscal.Domain

type Contato = private { Telefone: Telefone; Email: EmailAddress }

[<AutoOpen>]
module TomadorSharedUtils =
    let createContato telefone email =
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

        let createContato' telefone email =
            { Telefone = telefone; Email = email }

        createContato' <!> createTelefone telefone
        <*> createEmail email


    let createInscricaoMunicipalTomador inscricaoMunicipal mapErrors =
        StrMax15.createOptional inscricaoMunicipal
        |> mapFailuresR mapErrors
