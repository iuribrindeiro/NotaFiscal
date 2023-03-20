module NotaFiscal.Domain.ValidationErrors

type StringMaxLenError = MaxLen of int

type TelefoneErrors =
    | TelefoneNotEmpty
    | TelefoneMustBetweenLen of int * int
    | TelefoneDoesntMatchPattern

type MaxLenStringNotEmpty =
    | MaxLenNotEmpty
    | NotEmptyMaxLen of int

type MaxLenStringNotEmptyPattern =
    | MaxLenPatternNotEmpty
    | PatternNotEmptyMaxLen of int
    | DoesntMatchPattern

type StringLenNotEmpty =
    | LenNotEmpty
    | MustHaveLen of int


type UFErrors =
    | NotEmptyUF
    | DoesntUFMatchPattern

type EnderecoErrors =
    | RuaInvalida of MaxLenStringNotEmpty
    | NumeroInvalido of MaxLenStringNotEmpty
    | BairroInvalido of MaxLenStringNotEmpty
    | CodigoMunicipioInvalido of MaxLenStringNotEmpty
    | UFInvalida of UFErrors
    | CepInvalido of StringLenNotEmpty
    | ComplementoInvalido of StringMaxLenError

type ContatoErrors =
    | TelefoneInvalido of TelefoneErrors
    | EmailInvalido of MaxLenStringNotEmptyPattern

type CPFErrors =
    | NotEmptyCPF
    | DoesntMatchCPFPattern

type CNPJErrors =
    | NotEmptyCNPJ
    | DoesntMatchCNPJPattern

type PessoaFisicaErrors =
    | CPFInvalido of CPFErrors
    | PessoaFisicaInscricaoMunicipalInvalida of StringMaxLenError
    | PessoaFisicaEnderecoInvalido of EnderecoErrors
    | PessoaFisicaContatoInvalido of ContatoErrors

type PessoaJuridicaErrors =
    | PessoaJuridicaCNPJInvalido of CNPJErrors
    | RazaoSocialInvalida of MaxLenStringNotEmpty
    | PessoaJuridicaInscricaoMunicipalInvalida of StringMaxLenError
    | PessoaJuridicaEnderecoInvalido of EnderecoErrors
    | PessoaJuridicaContatoInvalido of ContatoErrors
    | ContatoNotEmpty
    //Internal Error
    | FailConvertEnderecoFromDb
    | FailConvertContatoFromDb

type TomadorErrors =
    | PessoaFisicaInvalida of PessoaFisicaErrors
    | PessoaJuridicaInvalida of PessoaJuridicaErrors
    | TomadorDeveSerFisicoJuricoOuEstrangeiro
    //Internal Error
    | FailToConvertTomadorFromDbError

type PrestadorErrors =
    | PrestadorCNPJInvalido of CNPJErrors
    | PrestadorInscricaoMunicipalInvalida of MaxLenStringNotEmpty

type ValorDinheiroErrors = | ValorDinheiroMustBePositive

type PercentageError = | MustBeBetween0And100

type IssErrors =
    | IssRetidoInvalido of ValorDinheiroErrors
    | IssNaoRetidoInvalido of ValorDinheiroErrors
    | TipoIssInvalido of string

type ValoresServicoErrors =
    | ValorServicoInvalido of ValorDinheiroErrors
    | ValorDeducoesInvalido of ValorDinheiroErrors
    | ValorPisInvalido of ValorDinheiroErrors
    | ValorCofinsInvalido of ValorDinheiroErrors
    | ValorInssInvalido of ValorDinheiroErrors
    | ValorIrInvalido of ValorDinheiroErrors
    | ValorCsllInvalido of ValorDinheiroErrors
    | IssInvalido of IssErrors
    | OutrasRetencoesInvalido of ValorDinheiroErrors
    | DescontoIncondicionadoInvalido of ValorDinheiroErrors
    | DescontoCondicionadoInvalido of ValorDinheiroErrors
    | AliquotaInvalida of PercentageError

type ServicoErros =
    | ValoresInvalidos of ValoresServicoErrors
    | ItemListaServicoInvalido of MaxLenStringNotEmpty
    | CodigoTributacaoMunicipioInvalido of StringMaxLenError
    | DiscriminacaoInvalida of MaxLenStringNotEmpty
    | ServicoCodigoMunicipioInvalido of MaxLenStringNotEmpty
    | CodigoCnaeInvalido of StringMaxLenError
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
    | PrestadorInvalido of PrestadorErrors
    | TomadorInvalido of TomadorErrors
    | ServicoInvalido of ServicoErros
    //Internal Error
    | NotaFiscalStatusInvalido of NotaFiscalStatusInvalido
