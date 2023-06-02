module NotaFiscal.WebApplication.RequestTypes

type ContatoRequestDto = { Telefone: string option; Email: string option }

and EnderecoRequestDto =
    { Rua: string option
      Numero: string option
      Complemento: string option
      Bairro: string option
      CodigoMunicipio: string option
      UF: string option
      Cep: string option }


and TomadorPessoaFisica =
    { Cpf: string option
      InscricaoMunicipal: string option
      Endereco: EnderecoRequestDto option
      Contato: ContatoRequestDto option }

and TomadorPessoaJuridica =
    { Cnpj: string option
      InscricaoMunicipal: string option
      RazaoSocial: string option
      Contato: ContatoRequestDto option
      Endereco: EnderecoRequestDto option }

and TomadorRequestDto =
    { Cpf: string option
      Cnpj: string option
      InscricaoMunicipal: string option
      Endereco: EnderecoRequestDto option
      Contato: ContatoRequestDto option
      Nome: string option
      Estrangeiro: bool option }

and RegimeEspecialTributacao =
    | MicroempresaMunicipal
    | Estimativa
    | SociedadeProfissionais
    | Cooperativa
    | MicroempreendedorIndividual
    | MicroempreendedorPequenoPorte

and NaturezaOperacao =
    | TributacaoMunicipio
    | TributacaoForaMunicipio
    | Isencao
    | Imune
    | ExigibilidadeSuspensa of NaturezaOperacaoExigibilidadeSuspensa

and NaturezaOperacaoExigibilidadeSuspensa =
    | DecisaoJudicial
    | ProcedimentoAdministrativo

and TipoRps =
    | Rps
    | NotaFiscalConjugadaMista
    | Cupom

type ValoresRequestDto =
    { Servicos: decimal option
      Deducoes: decimal option
      Pis: decimal option
      Cofins: decimal option
      Inss: decimal option
      Ir: decimal option
      Csll: decimal option
      ValorIss: decimal option
      IssRetido: bool option
      OutrasRetencoes: decimal option
      DescontoCondicionado: decimal option
      DescontoIncondicionado: decimal option
      Aliquota: float option }

type ServicoRequestDto =
    { Valores: ValoresRequestDto option
      ItemListaServico: string option
      CodigoTributacaoMunicipio: string option
      Discriminacao: string option
      CodigoMunicipio: string option
      CodigoCnae: string option
      NaturezaOperacao: int option
      RegimeEspecialTributacao: int option
      OptanteSimplesNacional: bool option
      IncentivadorCultural: bool option }