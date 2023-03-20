module NotaFiscal.Domain.NotaFiscalServico

open System
open NotaFiscalPrimitives
open ValidationErrors

type NotaFiscalServico =
    { Prestador: Prestador
      Tomador: Tomador
      Servico: Servico
      Status: NotaFiscalServicoStatus }

and NotaFiscalServicoStatus =
    | Pendente
    | AguardandoAutorizacao of AguardandoAutorizacaoStatus
    | Autorizada of NotaFiscalAutorizadaStatus
    | ErroAutorizacao of NotaFiscalErroStatus
    | Cancelada of NotaFiscalCanceladaStatus
    | SolicitandoCancelamento of NotaFiscalSolicitandoCancelamentoStatus
    | FalhaCancelamento of NotaFiscalFalhaCancelamentoStatus

and NotaSolicitadoEnvio = { DataEmissao: DataEmissao; Rps: Rps }

and AguardandoAutorizacaoStatus =
    | AguardandoEnvio of NotaSolicitadoEnvio
    | AguardandoAutorizacaoPrefeitura of NotaSolicitadoEnvio

and NotaFiscalAutorizadaStatus = NotaSolicitadoEnvio * NumeroNota
and NotaFiscalErroStatus = NotaSolicitadoEnvio * ErroComunicacao list
and NotaFiscalSolicitandoCancelamentoStatus = NotaSolicitadoEnvio * NumeroNota * CodigoCancelamento
and NotaFiscalCanceladaStatus = NotaSolicitadoEnvio * NumeroNota * CodigoCancelamento
and NotaFiscalFalhaCancelamentoStatus = NotaSolicitadoEnvio * NumeroNota * CodigoCancelamento * ErroComunicacao list

and CodigoMunicipio = CodigoMunicipio of StringMax7.Value

and Prestador =
    { Cnpj: CNPJ.Value
      InscricaoMunicipal: StringMax15.Value }

and Contato =
    { Telefone: Telefone.Value
      Email: EmailAddress.Value }

and Endereco =
    { Rua: StringMax120.Value
      Numero: StringMax60.Value
      Complemento: StringMax60.OptionalValue
      Bairro: StringMax60.Value
      CodigoMunicipio: CodigoMunicipio
      UF: UF.Value
      Cep: String8.Value }


and TomadorPessoaFisica =
    { Cpf: CPF.Value
      InscricaoMunicipal: StringMax15.OptionalValue
      Endereco: Endereco option
      Contato: Contato option }

and TomadorPessoaJuridica =
    { Cnpj: CNPJ.Value
      InscricaoMunicipal: StringMax15.OptionalValue
      RazaoSocial: StringMax115.Value
      Contato: Contato
      Endereco: Endereco }

and TomadorEstrangeiro = TomadorEstrangeiro

and Tomador =
    | PessoaFisica of TomadorPessoaFisica option
    | PessoaJuridica of TomadorPessoaJuridica
    | Estrangeiro of TomadorEstrangeiro

and Rps =
    { Numero: int
      Serie: string
      Tipo: TipoRps }

and TipoRps =
    | Rps
    | NotaFiscalConjugadaMista
    | Cupom

and Servico =
    { Valores: Valores
      ItemListaServico: StringMax7.Value
      CodigoTributacaoMunicipio: StringMax20.OptionalValue
      Discriminacao: StringMax2000.Value
      CodigoMunicipioPrestacao: CodigoMunicipio
      CodigoCnae: StringMax7.OptionalValue
      NaturezaOperacao: NaturezaOperacao.Value
      RegimeEspecialTributacao: RegimeEspecialTributacao.Value
      OptanteSimplesNacional: bool
      IncentivadorCultural: bool }

and Iss =
    | Retido of ValorDinheiro.Value
    | NaoRetido of ValorDinheiro.Value

and Valores =
    { Servicos: ValorDinheiro.Value
      Deducoes: ValorDinheiro.OptionalValue
      Pis: ValorDinheiro.OptionalValue
      Cofins: ValorDinheiro.OptionalValue
      Inss: ValorDinheiro.OptionalValue
      Ir: ValorDinheiro.OptionalValue
      Csll: ValorDinheiro.OptionalValue
      Iss: Iss
      OutrasRetencoes: ValorDinheiro.OptionalValue
      DescontoCondicionado: ValorDinheiro.OptionalValue
      DescontoIncondicionado: ValorDinheiro.OptionalValue
      Aliquota: Percentage.OptionalValue }

and CodigoCancelamento = string

and DataEmissao = DateTime

and NumeroNota = int

and NumeroLote = string

and CodigoErro = string

and ErroComunicacao =
    { CodigoMensagemAlerta: CodigoErro
      Mensagem: string
      Correcao: string option }

and Correcao = string

let createCnpj cnpj mapErrors =
    cnpj |> CNPJ.create |> mapFailures mapErrors

let createCnpjPrestador cnpj = createCnpj cnpj PrestadorCNPJInvalido

let createInscricaoMunicipalPrestador inscricaoMunicipal =
    inscricaoMunicipal
    |> StringMax15.create
    |> mapFailures PrestadorInscricaoMunicipalInvalida


let createPrestador cnpj inscricaoMunicipal =
    { Cnpj = cnpj
      InscricaoMunicipal = inscricaoMunicipal }

let private createNotaFiscalServico' prestador tomador servico =
    { Prestador = prestador
      Tomador = tomador
      Servico = servico
      Status = Pendente }

let createNotaFiscalServico prestadorResult tomadorResult servicoResult =
    createNotaFiscalServico' <!> (prestadorResult |> mapFailures PrestadorInvalido)
    <*> (tomadorResult |> mapFailures TomadorInvalido)
    <*> (servicoResult |> mapFailures ServicoInvalido)
