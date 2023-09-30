[<Microsoft.FSharp.Core.AutoOpen>]
module NotaFiscal.Domain.DomainEvents

open System

type EnderecoError =
    //Rua
    | RuaIsRequired
    | RuaMustNotBeMoreThan120Chars
    //Numero
    | NumeroIsRequired
    | NumeroMustNotBeMoreThan60Chars
    //Bairro
    | BairroIsRequired
    | BairroMustNotBeMoreThan60Chars
    //Codigo Municipio
    | CodigoMunicipioIsRequired
    | CodigoMunicipioMustNotBeMoreThan7Chars
    //Uf
    | UfIsRequired
    | UfMustBe2Chars
    //Cep
    | CepIsRequired
    | CepMustBe8Chars
    //Complemento
    | ComplementoMustNotBeMoreThan60Chars

type ContatoError =
    //Telefone
    | TelefoneIsRequired
    | TelefoneMustBetween8And11Len
    | TelefoneDoesntMatchPattern
    //Email
    | EmailIsRequired
    | EmailMustNotBeMoreThan80Chars
    | EmailDoesntMatchPattern

type PessoaFisicaErrors =
    //CPF
    | CPFIsRequired
    | CPFDoesntMatchPattern

    //Nome
    | NomeMustNotBeMoreThan115Chars

    //Inscricao Municipal
    | InscricaoMunicipalMustNotBeMoreThan15Chars
    | EnderecoInvalido of EnderecoError
    | ContatoInvalido of ContatoError

type PessoaJuridicaErrors =
    //CNPJ
    | CNPJIsRequired
    | CNPJDoesntMatchPattern
    //Razao Social
    | RazaoSocialIsRequired
    | RazaoSocialMustNotBeMoreThan115Chars
    //Inscricao Municipal
    | InscricaoMunicipalMustNotBeMoreThan15Chars
    //Endereco
    | EnderecoIsRequired
    | EnderecoInvalido of EnderecoError
    //Contato
    | ContatoIsRequired
    | ContatoInvalido of ContatoError
    | FailConvertEnderecoFromDb
    | FailConvertContatoFromDb

type TomadorEstrangeiroErrors =
    | NomeMustNotBeMoreThan115Chars
    | EnderecoInvalido of EnderecoError
    | ContatoInvalido of ContatoError

type TomadorErrors =
    | PessoaFisicaInvalida of PessoaFisicaErrors
    | PessoaJuridicaInvalida of PessoaJuridicaErrors
    | TomadorEstrangeiroInvalido of TomadorEstrangeiroErrors


type IssErrors =
    | IssRetidoInvalido
    | IssNaoRetidoInvalido
    | TipoIssInvalido of string

type ValoresServicoErrors =
    | ValorServicoInvalido
    | ValorDeducoesInvalido
    | ValorPisInvalido
    | ValorCofinsInvalido
    | ValorInssInvalido
    | ValorIrInvalido
    | ValorCsllInvalido
    | IssInvalido of IssErrors
    | OutrasRetencoesInvalido
    | DescontoIncondicionadoInvalido
    | DescontoCondicionadoInvalido
    | AliquotaInvalida

type ServicoErros =
    | ValoresIsRequired
    | ValoresInvalidos of ValoresServicoErrors
    | ItemListaServicoIsRequired
    | ItemListaServicoMustNotBeMoreThan7Chars
    | CodigoTributacaoMunicipioMustNotBeMoreThan20Chars
    | DiscriminacaoIsRequired
    | DiscriminacaoMustNotBeMoreThan2000Chars
    | CodigoMunicipioIsRequired
    | CodigoMunicipioMustNotBeMoreThan7Chars
    | CodigoCnaeMustNotBeMoreThan7Chars
    | NaturezaOperacaoInvalida
    | RegimeEspecialTributacaoInvalido

type RpsErrors =
    | NumeroIsRequired
    | NumeroMustBePositive
    | SerieIsRequired
    | SerieMustNotBeMoreThan5Chars
    | TipoInvalid
    | RpsIsRequired


type SolicitandoEmissaoErrors =
    | RpsInvalido of RpsErrors
    | DataEmissaoIsRequired
    | DataEmissaoMustBeLessThanNow
    | NumeroLoteMustBePositive

type AguardandoAutorizacaoErrors =
    | NumeroLoteMustBePositive
    | ProtocoloIsRequired
    | DataEmissaoMustBeLessThanNow
    | RpsInvalido of RpsErrors

type AutorizadaErrors =
    | NumeroNotaMustBePositive
    | DataEmissaoMustBeLessThanNow
    | RpsInvalido of RpsErrors


type ErroComunicacaoValidationErrs =
    | MensagemIsRequired
    | CodigoMensagemIsRequired

type ErroEmissaoErrors =
    | DataEmissaoMustBeLessThanNow
    | RpsInvalido of RpsErrors
    | ErrosComunicacao of ErroComunicacaoValidationErrs


type SolicitandoCancelamentoErrors =
    | NumeroNotaMustBePositive
    | RpsInvalido of RpsErrors
    | DataEmissaoMustBeLessThanNow
    | CodigoCancelamentoIsRequired

type CanceladaErros =
    | NumeroNotaMustBePositive
    | RpsInvalido of RpsErrors
    | DataEmissaoMustBeLessThanNow
    | CodigoCancelamentoIsRequired
    | DataCancelamentoMustBeLessOrEqualThanNow

type StatusNotaInvalidoErrs =
    | SolicitandoEmissaoErrors of SolicitandoEmissaoErrors
    | AguardandoAutorizacaoErrors of AguardandoAutorizacaoErrors
    | AutorizadaErrors of AutorizadaErrors
    | ErroEmissaoErrors of ErroEmissaoErrors
    | SolicitandoCancelamentoErrors of SolicitandoCancelamentoErrors
    | CanceladaErros of CanceladaErros


type ValidationError =
    | TomadorIsRequired
    | TomadorInvalido of TomadorErrors
    | ServicoIsRequired
    | ServicoInvalido of ServicoErros
    | StatusNotaInvalido of StatusNotaInvalidoErrs

type DatabaseError =
    | NotFound of string * string
    | FailedCommunicateDb of Exception
    | FailedDeserializeFromDb of ValidationError

type Errors =
    | ValidationError of ValidationError
    | DatabaseError of DatabaseError

type DomainEvents = | NotaFiscalCriada of Guid

type DomainMessages =
    | Erros of Errors
    | DomainEvents of DomainEvents
