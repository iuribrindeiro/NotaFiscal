module NotaFiscal.WebApplication.RequestTypes

type NotaFiscalServico =
    { Prestador: Prestador
      Tomador: Tomador
      Servico: Servico }

and InscricaoMunicipal = string option

and CodigoMunicipio = string

and Prestador =
    { Cnpj: string
      InscricaoMunicipal: InscricaoMunicipal }

and Contato = { Telefone: string; Email: string }

and Endereco =
    { Rua: string
      Numero: string
      Complemento: string option
      Bairro: string
      CodigoMunicipio: CodigoMunicipio
      UF: string
      Cep: string }


and TomadorPessoaFisica =
    { Cpf: string
      InscricaoMunicipal: InscricaoMunicipal
      Endereco: Endereco option
      Contato: Contato option }

and TomadorPessoaJuridica =
    { Cnpj: string
      InscricaoMunicipal: InscricaoMunicipal
      RazaoSocial: string
      Contato: Contato
      Endereco: Endereco }

and Tomador =
    | PessoaFisica of TomadorPessoaFisica option
    | PessoaJuridica of TomadorPessoaJuridica
    | Estrangeiro

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

and Servico =
    { Valores: Valores
      ItemListaServico: string
      CodigoTributacaoMunicipio: string option
      Discriminacao: string
      CodigoMunicipio: CodigoMunicipio
      CodigoCnae: string
      NaturezaOperacao: NaturezaOperacao
      RegimeEspecialTributacao: RegimeEspecialTributacao
      OptanteSimplesNacional: bool
      IncentivadorCultural: bool }

and Iss =
    | Retido of decimal
    | NaoRetido of decimal

and Valores =
    { Servicos: decimal
      Deducoes: decimal option
      Pis: decimal option
      Cofins: decimal option
      Inss: decimal option
      Ir: decimal option
      Csll: decimal option
      Iss: Iss
      OutrasRetencoes: decimal option
      DescontoCondicionado: decimal option
      DescontoIncondicionado: decimal option
      Aliquota: float option }
