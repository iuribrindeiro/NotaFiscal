﻿module NotaFiscal.WebApplication.RequestTypes

open System
open NotaFiscal.Domain.NotaFiscalServico

type DadosNotaFiscalServicoViewModel =
    { Id: Guid option
      DataCriacao: DateTime option
      DataAlteracao: DateTime option
      Prestador: PrestadorViewModel option
      Tomador: TomadorViewModel option
      Servico: ServicoViewModel option }

and PrestadorViewModel =
    { Cnpj: string option
      InscricaoMunicipal: InscricaoMunicipal option }

and ContatoViewModel = { Telefone: string; Email: string }

and EnderecoViewModel =
    { Rua: Rua
      Numero: Numero
      Complemento: Complemento
      Bairro: Bairro
      CodigoMunicipio: CodigoMunicipio
      UF: string
      Cep: Cep }


and TomadorPessoaFisicaViewModel =
    { Cpf: string
      InscricaoMunicipal: string option
      Contato: ContatoViewModel option
      Endereco: EnderecoViewModel option }

and TomadorPessoaJuridicaViewModel =
    { Cnpj: string
      RazaoSocial: string
      InscricaoMunicipal: string option
      Contato: ContatoViewModel
      Endereco: EnderecoViewModel }

and TomadorViewModel =
    | PessoaFisicaVM of TomadorPessoaFisicaViewModel option
    | PessoaJuridicaVM of TomadorPessoaJuridicaViewModel
    | EstrangeiroVM


and RegimeEspecialTributacaoViewModel =
    | MicroempresaMunicipal = 1
    | Estimativa = 2
    | SociedadeProfissionais = 3
    | Cooperativa = 4
    | MicroempreendedorIndividual = 5
    | MicroempreendedorPequenoPorte = 6

and NaturezaOperacaoViewModel =
    | TributacaoMunicipio = 1
    | TributacaoForaMunicipio = 2
    | Isencao = 3
    | Imune = 4
    | DecisaoJudicial = 5
    | ProcedimentoAdministrativo = 6

and ServicoViewModel =
    { Valores: ValoresViewModel option
      ItemListaServico: string option
      CodigoTributacaoMunicipio: string option
      Discriminacao: string option
      MunicipioPrestacaoServico: string option
      NaturezaOperacao: NaturezaOperacaoViewModel
      RegimeEspecialTributacao: RegimeEspecialTributacaoViewModel
      OptanteSimplesNacional: bool option
      IncentivadorCultural: bool option }


and ValoresViewModel =
    { Servicos: decimal option
      Deducoes: decimal option
      Pis: decimal option
      Cofins: decimal option
      Inss: decimal option
      Ir: decimal option
      Csll: decimal option
      IssRetido: bool option
      ValorIssRetido: decimal option
      ValorIss: decimal option
      OutrasRetencoes: decimal option
      BaseCalculo: decimal option
      DescontoCondicionado: decimal option
      DescontoIncondicionado: decimal option
      Aliquota: float<percentage> option
      ValorLiquidoNfse: decimal option }