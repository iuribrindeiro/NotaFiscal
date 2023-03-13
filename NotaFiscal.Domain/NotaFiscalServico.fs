module NotaFiscalServico

open System

[<Measure>]
type percentage


type NotaFiscalServico =
    | Pendente of DadosNotaFiscalServico
    | AguardandoAutorizacao of NotaEnviada
    | Autorizada of NotaAutorizada
    | Erro of NotaErro
    | Cancelada of NotaCancelada

and NotaEnviada = DadosNotaFiscalServico * DataEmissao * Rps
and NotaAutorizada = NotaEnviada * NumeroNota
and NotaErro = NotaEnviada * MensagemRetorno list
and NotaCancelada = NotaAutorizada * CodigoCancelamento

and DadosNotaFiscalServico =
    { Id: Guid
      DataCriacao: DateTime
      DataAlteracao: DateTime option
      Prestador: Prestador
      Tomador: Tomador
      Servico: Servico }

and CodigoCancelamento = string

and DataEmissao = DateTime

and NumeroNota = string

and NumeroLote = string

and CodigoMensagemAlerta = string

and DadosMensagemRetorno =
    { Rps: Rps
      CodigoMensagemAlerta: CodigoMensagemAlerta
      Mensagem: string }

and Correcao = string

and MensagemRetorno =
    | Sucesso of DadosMensagemRetorno
    | Falha of DadosMensagemRetorno * Correcao


and InscricaoMunicipal = int

and Prestador =
    { Cnpj: string
      InscricaoMunicipal: InscricaoMunicipal option }

and Contato = { Telefone: string; Email: string }

and UF =
    | AC
    | AL
    | AM
    | AP
    | BA
    | CE
    | DF
    | ES
    | GO
    | MA
    | MG
    | MS
    | MT
    | PA
    | PB
    | PE
    | PI
    | PR
    | RJ
    | RN
    | RO
    | RR
    | RS
    | SC
    | SE
    | SP
    | TO


and Rua = string

and Numero = string

and Complemento = string option
and Bairro = string
and CodigoMunicipio = string
and Cep = string

and Endereco =
    { Rua: Rua
      Numero: Numero
      Complemento: Complemento
      Bairro: Bairro
      CodigoMunicipio: CodigoMunicipio
      UF: UF
      Cep: Cep }


and DadosTomadorPessoaFisica =
    { Cpf: string
      InscricaoMunicipal: InscricaoMunicipal option
      Endereco: Endereco option
      Contato: Contato option }

and DadosTomadorPessoaJuridica =
    { Cnpj: string
      InscricaoMunicipal: InscricaoMunicipal option
      RazaoSocial: string
      Contato: Contato
      Endereco: Endereco }

and Tomador =
    | PessoaFisica of DadosTomadorPessoaFisica option
    | PessoaJuridica of DadosTomadorPessoaJuridica
    | Estrangeiro

and RegimeEspecialTributacao =
    | MicroempresaMunicipal of Descricao
    | Estimativa of Descricao
    | SociedadeProfissionais of Descricao
    | Cooperativa of Descricao
    | MicroempreendedorIndividual of Descricao
    | MicroempreendedorPequenoPorte of Descricao

and NaturezaOperacao =
    | TributacaoMunicipio of Descricao
    | TributacaoForaMunicipio of Descricao
    | Isencao of Descricao
    | Imune of Descricao
    | ExigibilidadeSuspensa of NaturezaOperacaoExigibilidadeSuspensa

and NaturezaOperacaoExigibilidadeSuspensa =
    | DecisaoJudicial of Descricao
    | ProcedimentoAdministrativo of Descricao

and Rps =
    { Numero: uint32
      Serie: uint32
      Tipo: TipoRps }

and Descricao = string

and TipoRps =
    | Rps of Descricao
    | NotaFiscalConjugadaMista of Descricao
    | Cupom of Descricao

and Servico =
    { Valores: Valores
      ItemListaServico: string
      CodigoTributacaoMunicipio: string
      Discriminacao: string
      MunicipioPrestacaoServico: string
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
      BaseCalculo: decimal option
      DescontoCondicionado: decimal option
      DescontoIncondicionado: decimal option
      Aliquota: float<percentage> option
      ValorLiquidoNfse: decimal option }


//Esses mappers possivelmente serao movidos para o projeto de integracao com as prefeituras
let mapNaturezaOperacao naturezaOperacao =
    match naturezaOperacao with
    | TributacaoMunicipio _ -> 1
    | TributacaoForaMunicipio _ -> 2
    | Isencao _ -> 3
    | Imune _ -> 4
    | ExigibilidadeSuspensa motivo ->
        match motivo with
        | DecisaoJudicial _ -> 5
        | ProcedimentoAdministrativo _ -> 6

let mapRegimeEspecialTributacao regime =
    match regime with
    | MicroempresaMunicipal _ -> 1
    | Estimativa _ -> 2
    | SociedadeProfissionais _ -> 3
    | Cooperativa _ -> 4
    | MicroempreendedorIndividual _ -> 5
    | MicroempreendedorPequenoPorte _ -> 6

//Os campos booleanos nas prefeituras geralmente sao mapeados para 1(true)/2(false)
let mapBool verdadeiro =
    match verdadeiro with
    | true -> 1
    | false -> 2

let mapTipoRps tipo =
    match tipo with
    | Rps _ -> 1
    | NotaFiscalConjugadaMista _ -> 2
    | Cupom _ -> 3


let ufFromString (s: string) : UF option =
    match s with
    | "AC" -> Some AC
    | "AL" -> Some AL
    | "AP" -> Some AP
    | "AM" -> Some AM
    | "BA" -> Some BA
    | "CE" -> Some CE
    | "DF" -> Some DF
    | "ES" -> Some ES
    | "GO" -> Some GO
    | "MA" -> Some MA
    | "MT" -> Some MT
    | "MS" -> Some MS
    | "MG" -> Some MG
    | "PA" -> Some PA
    | "PB" -> Some PB
    | "PR" -> Some PR
    | "PE" -> Some PE
    | "PI" -> Some PI
    | "RJ" -> Some RJ
    | "RN" -> Some RN
    | "RS" -> Some RS
    | "RO" -> Some RO
    | "RR" -> Some RR
    | "SC" -> Some SC
    | "SP" -> Some SP
    | "SE" -> Some SE
    | "TO" -> Some TO
    | _ -> None
