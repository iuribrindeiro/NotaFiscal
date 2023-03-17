module NotaFiscal.Domain.NotaFiscalServico

open System
open NotaFiscalPrimitives


type NotaFiscalServico =
    { Prestador: Prestador
      Tomador: Tomador
      Servico: Servico
      Status: NotaFiscalServicoStatus }

and NotaFiscalServicoStatus =
    | Pendente
    | AguardandoAutorizacao of NotaFiscalEnviadaStatus
    | Autorizada of NotaFiscalAutorizadaStatus
    | Erro of NotaFiscalErroStatus
    | Cancelada of NotaFiscalCanceladaStatus

and NotaFiscalEnviadaStatus =
    | AguardandoEnvio of DataEmissao * Rps
    | AguardandoAutorizacao of DataEmissao * Rps

and NotaFiscalAutorizadaStatus = NotaFiscalEnviadaStatus * NumeroNota
and NotaFiscalErroStatus = NotaFiscalEnviadaStatus * StatusMensagemRetorno list
and NotaFiscalCanceladaStatus = NotaFiscalAutorizadaStatus * CodigoCancelamento

and InscricaoMunicipal = StringMax15.Value option

and CodigoMunicipio = StringMax7.Value

and Prestador =
    { Cnpj: CNPJ.Value
      InscricaoMunicipal: InscricaoMunicipal }

and Contato =
    { Telefone: StringMax11.Value
      Email: StringMax80.Value }

and Endereco =
    { Rua: StringMax120.Value
      Numero: StringMax60.Value
      Complemento: StringMax60.Value option
      Bairro: StringMax60.Value
      CodigoMunicipio: CodigoMunicipio
      UF: UF.Value
      Cep: String8.Value }


and TomadorPessoaFisica =
    { Cpf: CPF.Value
      InscricaoMunicipal: InscricaoMunicipal
      Endereco: Endereco option
      Contato: Contato option }

and TomadorPessoaJuridica =
    { Cnpj: CNPJ.Value
      InscricaoMunicipal: InscricaoMunicipal
      RazaoSocial: StringMax115.Value
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

and Rps =
    { Numero: uint32
      Serie: uint32
      Tipo: TipoRps }

and TipoRps =
    | Rps
    | NotaFiscalConjugadaMista
    | Cupom

and Servico =
    { Valores: Valores
      ItemListaServico: StringMax7.Value
      CodigoTributacaoMunicipio: StringMax20.Value option
      Discriminacao: StringMax2000.Value
      CodigoMunicipio: CodigoMunicipio
      CodigoCnae: StringMax7.Value
      NaturezaOperacao: NaturezaOperacao
      RegimeEspecialTributacao: RegimeEspecialTributacao
      OptanteSimplesNacional: bool
      IncentivadorCultural: bool }

and Iss =
    | Retido of Dinheiro.Value
    | NaoRetido of Dinheiro.Value

and Valores =
    { Servicos: Dinheiro.Value
      Deducoes: Dinheiro.Value option
      Pis: Dinheiro.Value option
      Cofins: Dinheiro.Value option
      Inss: Dinheiro.Value option
      Ir: Dinheiro.Value option
      Csll: Dinheiro.Value option
      Iss: Iss
      OutrasRetencoes: Dinheiro.Value option
      DescontoCondicionado: Dinheiro.Value option
      DescontoIncondicionado: Dinheiro.Value option
      Aliquota: Percentage.Value option }

and CodigoCancelamento = string

and DataEmissao = DateTime

and NumeroNota = string

and NumeroLote = string

and CodigoMensagemAlerta = string

and MensagemRetorno =
    { Rps: Rps
      CodigoMensagemAlerta: CodigoMensagemAlerta
      Mensagem: string }

and Correcao = string

and StatusMensagemRetorno =
    | Sucesso of MensagemRetorno
    | Falha of MensagemRetorno * Correcao



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


// let calcularValorLiquidoTotal (valores: Valores) =
//     let valorLiquidoTotal =
//         valores.Servicos
//         - (valores.Deducoes |> Option.defaultValue 0m)
//         - (valores.Pis |> Option.defaultValue 0m)
//         - (valores.Cofins |> Option.defaultValue 0m)
//         - (valores.Inss |> Option.defaultValue 0m)
//         - (valores.Ir |> Option.defaultValue 0m)
//         - (valores.Csll |> Option.defaultValue 0m)
//         - (valores.OutrasRetencoes |> Option.defaultValue 0m)
//         - (valores.DescontoCondicionado |> Option.defaultValue 0m)
//         - (valores.DescontoIncondicionado |> Option.defaultValue 0m)

//     { valores with
//         ValorLiquidoTotal = Some valorLiquidoTotal }

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
