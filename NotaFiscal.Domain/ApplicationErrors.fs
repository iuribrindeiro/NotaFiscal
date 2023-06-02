module NotaFiscal.Domain.ApplicationErrors

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

type TomadorErrors =
    | PessoaFisicaInvalida of PessoaFisicaErrors
    | PessoaJuridicaInvalida of PessoaJuridicaErrors


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


//Internal Error
type NotaFiscalStatusInvalido =
    | DataEmissaoVazia
    | NumeroRpsInvalido
    | NumeroRpsVazio
    | TipoRpsVazio
    | TipoRpsInvalido
    | SerieRpsVazia
    | SerieRpsInvalida
    | NumeroProtocoloVazio
    | NumeroNotaInvalido
    | CodigoErroComunicacaoVazio
    | MensagemAlertaVazia
    | CorrecaoInvalida
    | CodigoCancelamentoInvalido
    | CodigoCancelamentoVazio
    | FailConvertMensagemRetornoFromDb
    | EmptyErroComunicacaoFromDb
    | InvalidStatus

type NotaFiscalErrors =
    | TomadorIsRequired
    | TomadorInvalido of TomadorErrors
    | ServicoIsRequired
    | ServicoInvalido of ServicoErros
    //Internal Error
    | NotaFiscalStatusInvalido of NotaFiscalStatusInvalido
    | FailToConvertTomadorFromDbError
    | FailToConvertContatoFromDbError
    | FailToConvertEnderecoFromDbError
    | FailToSaveNotaDb of exn
    | FailToGetNotasFromDb of exn