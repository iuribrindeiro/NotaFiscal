module NotaFiscal.Domain.Dto

open System
open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.Tomador
open NotaFiscalPrimitives
open Endereco
open NotaFiscal.Domain.Servico.ValoresServico
open NotaFiscal.Domain.Servico.Servico
open NotaFiscalStatus

type NotaFiscalServicoDto =
    { Id: Guid; Tomador: TomadorDto; Servico: ServicoDto; Status: StatusDto }

and ServicoDto =
    { Valores: ValoresDto
      ItemListaServico: string
      CodigoTributacaoMunicipio: string option
      Discriminacao: string
      CodigoMunicipioPrestacao: string
      CodigoCnae: string option
      NaturezaOperacao: int
      RegimeEspecialTributacao: int
      OptanteSimplesNacional: bool
      IncentivadorCultural: bool }

and TomadorDto =
    { Discriminator: string
      Cpf: string option
      Cnpj: string option
      InscricaoMunicipal: string option
      Nome: string option
      Endereco: EnderecoDTO option
      Contato: ContatoDTO option }

and StatusDto =
    { Discriminator: string
      NumeroLote: string option
      NumeroProtocolo: string option
      DataCancelamento: DateTime option
      NumeroNota: string option
      DataEmissao: DateTime option
      ErrosComunicacao: ErroComunicacaoDTO list option
      Rps: RpsDTO option
      CodigoCancelamento: string option }

and ErroComunicacaoDTO =
    { Id: Guid
      CodigoMensagemAlerta: string
      Mensagem: string
      Correcao: string option }

and RpsDTO = { Numero: int; Serie: string; Tipo: int }

and EnderecoDTO =
    { Rua: string
      Numero: string
      Complemento: string option
      Bairro: string
      CodigoMunicipio: string
      Cep: string }

and ContatoDTO = { Telefone: string; Email: string }


and ValoresDto =
    { Servicos: decimal
      Deducoes: decimal option
      Pis: decimal option
      Cofins: decimal option
      Inss: decimal option
      Ir: decimal option
      Csll: decimal option
      ValorIssRetido: decimal
      ValorIssNaoRetido: decimal
      OutrasRetencoes: decimal option
      DescontoCondicionado: decimal option
      DescontoIncondicionado: decimal option
      Aliquota: double option }

module ServicoDto =
    let mapValorIssRetido (iss: Iss) =
        match iss with
        | Retido r -> Dinheiro.mapToValue r
        | NaoRetido _ -> 0m

    let mapValorIssNaoRetido (iss: Iss) =
        match iss with
        | Retido _ -> 0m
        | NaoRetido nr -> Dinheiro.mapToValue nr


    let fromValoresDomain (valores: Valores) : ValoresDto =
        { Servicos = valores.Servicos |> Dinheiro.mapToValue
          Deducoes = valores.Deducoes |> Dinheiro.mapToValueOptional
          Pis = valores.Pis |> Dinheiro.mapToValueOptional
          Cofins = valores.Cofins |> Dinheiro.mapToValueOptional
          Inss = valores.Inss |> Dinheiro.mapToValueOptional
          Ir = valores.Ir |> Dinheiro.mapToValueOptional
          Csll = valores.Csll |> Dinheiro.mapToValueOptional
          ValorIssRetido = mapValorIssRetido valores.Iss
          ValorIssNaoRetido = mapValorIssNaoRetido valores.Iss
          OutrasRetencoes =
            valores.OutrasRetencoes |> Dinheiro.mapToValueOptional
          DescontoCondicionado =
            valores.DescontoCondicionado |> Dinheiro.mapToValueOptional
          DescontoIncondicionado =
            valores.DescontoIncondicionado
            |> Dinheiro.mapToValueOptional
          Aliquota = valores.Aliquota |> Percentage.mapToValueOptional }

    let fromServicoDomain (servico: Servico) : ServicoDto =
        { Valores = fromValoresDomain servico.Valores
          ItemListaServico = servico.ItemListaServico |> StrMax7.mapToValue
          CodigoTributacaoMunicipio =
            servico.CodigoTributacaoMunicipio
            |> StrMax20.mapToValueOption
          Discriminacao = servico.Discriminacao |> StrMax2000.mapToValue
          CodigoMunicipioPrestacao =
            servico.CodigoMunicipioPrestacao |> StrMax7.mapToValue
          CodigoCnae = servico.CodigoCnae |> StrMax7.mapToValueOptional
          NaturezaOperacao =
            servico.NaturezaOperacao |> NaturezaOperacao.mapValue
          RegimeEspecialTributacao =
            servico.RegimeEspecialTributacao
            |> RegimeEspecialTributacao.mapValue
          OptanteSimplesNacional = servico.OptanteSimplesNacional
          IncentivadorCultural = servico.IncentivadorCultural }

module TomadorDto =
    let mapEnderecoFromDomain (endereco: Endereco) =
        { Rua = endereco.Rua |> StrMax120.mapToValue
          Numero = endereco.Numero |> StrMax60.mapToValue
          Complemento = endereco.Complemento |> StrMax60.mapToValueOptional
          Bairro = endereco.Bairro |> StrMax60.mapToValue
          CodigoMunicipio = endereco.CodigoMunicipio |> StrMax7.mapToValue
          Cep = endereco.Cep |> StrOf8.mapToValue }

    let mapContatoFromDomain (contato: Contato) =
        { Email = contato.Email |> EmailAddress.mapToValue
          Telefone = contato.Telefone |> Telefone.mapValue }

    let fromTomadorPessoaFisica (tomador: TomadorPessoaFisica option) =
        match tomador with
        | Some pf ->
            { Discriminator = nameof (PessoaFisica)
              Cpf = CPF.mapToValue pf.Cpf |> Some
              Nome = pf.Nome |> StrMax115.mapToValueOptional
              Endereco = Option.map mapEnderecoFromDomain pf.Endereco
              Contato = Option.map mapContatoFromDomain pf.Contato
              Cnpj = None
              InscricaoMunicipal = None }
        | None ->
            { Discriminator = nameof (PessoaFisica)
              Cnpj = None
              Cpf = None
              InscricaoMunicipal = None
              Nome = None
              Endereco = None
              Contato = None }

    let fromTomadorPessoaJuridica (tomador: TomadorPessoaJuridica) =
        { Discriminator = nameof (PessoaJuridica)
          Cnpj = CNPJ.mapToValue tomador.Cnpj |> Some
          Cpf = None
          InscricaoMunicipal =
            tomador.InscricaoMunicipal |> StrMax15.mapToValueOptional
          Nome = StrMax115.mapToValue tomador.RazaoSocial |> Some
          Endereco = mapEnderecoFromDomain tomador.Endereco |> Some
          Contato = mapContatoFromDomain tomador.Contato |> Some }

    let fromTomadorEstrangeiro tomador =
        { Discriminator = nameof (Estrangeiro)
          Cnpj = None
          Cpf = None
          InscricaoMunicipal = None
          Nome = StrMax115.mapToValueOptional tomador.RazaoSocial
          Endereco = Option.map mapEnderecoFromDomain tomador.Endereco
          Contato = Option.map mapContatoFromDomain tomador.Contato }

    let fromTomadorDomain (tomador: Tomador) : TomadorDto =
        match tomador with
        | PessoaFisica t -> fromTomadorPessoaFisica t
        | PessoaJuridica pj -> fromTomadorPessoaJuridica pj
        | Estrangeiro es -> fromTomadorEstrangeiro es


module StatusDto =
    let fromRpsDomain (rps: Rps) : RpsDTO =
        let tipoRpsDto =
            match rps.Tipo with
            | Rps -> 1
            | NotaFiscalConjugadaMista -> 2
            | Cupom -> 3

        { Numero = rps.Numero; Serie = rps.Serie; Tipo = tipoRpsDto }

    let fromErrosComunicacaoDomain
        (errorsComunicacao: ErroComunicacao list)
        : ErroComunicacaoDTO list
        =
        errorsComunicacao
        |> List.map (fun (errCmn) ->
            { Id = errCmn.Id
              CodigoMensagemAlerta = errCmn.CodigoMensagemAlerta
              Mensagem = errCmn.Mensagem
              Correcao = errCmn.Correcao })

    let fromStatusDomain (status: NotaFiscalServicoStatus) : StatusDto =
        match status with
        | Pendente ->
            { Discriminator = nameof Pendente
              NumeroLote = None
              NumeroProtocolo = None
              NumeroNota = None
              DataEmissao = None
              ErrosComunicacao = None
              Rps = None
              CodigoCancelamento = None
              DataCancelamento = None }
        | SolicitandoEmissao(numeroLote, dataEmissao, rps) ->
            { Discriminator = nameof SolicitandoEmissao
              NumeroLote = Some numeroLote
              NumeroProtocolo = None
              NumeroNota = None
              DataEmissao = Some dataEmissao
              ErrosComunicacao = None
              Rps = fromRpsDomain rps |> Some
              CodigoCancelamento = None
              DataCancelamento = None }
        | AguardandoAutorizacao(numeroLote, protocolo, dataEmissao, rps) ->
            { Discriminator = nameof AguardandoAutorizacao
              NumeroLote = Some numeroLote
              NumeroProtocolo = Some protocolo
              NumeroNota = None
              DataEmissao = Some dataEmissao
              ErrosComunicacao = None
              Rps = fromRpsDomain rps |> Some
              CodigoCancelamento = None
              DataCancelamento = None }
        | Autorizada(dataEmissao, rps, numeroNota) ->
            { Discriminator = nameof Autorizada
              NumeroLote = None
              NumeroProtocolo = None
              NumeroNota = Some numeroNota
              DataEmissao = Some dataEmissao
              ErrosComunicacao = None
              Rps = fromRpsDomain rps |> Some
              CodigoCancelamento = None
              DataCancelamento = None }
        | ErroAutorizacao(dataEmissao, rps, errosComunicacao) ->
            { Discriminator = nameof ErroAutorizacao
              NumeroLote = None
              NumeroProtocolo = None
              NumeroNota = None
              DataEmissao = Some dataEmissao
              ErrosComunicacao =
                fromErrosComunicacaoDomain errosComunicacao |> Some
              Rps = fromRpsDomain rps |> Some
              CodigoCancelamento = None
              DataCancelamento = None }
        | SolicitandoCancelamento(dataEmissao,
                                  rps,
                                  numeroNota,
                                  codigoCancelamento) ->
            { Discriminator = nameof SolicitandoCancelamento
              NumeroLote = None
              NumeroProtocolo = None
              NumeroNota = Some numeroNota
              DataEmissao = Some dataEmissao
              ErrosComunicacao = None
              Rps = fromRpsDomain rps |> Some
              CodigoCancelamento = Some codigoCancelamento
              DataCancelamento = None }
        | Cancelada(dataEmissao,
                    rps,
                    numeroNota,
                    codigoCancelamento,
                    dataCancelamento) ->
            { Discriminator = nameof Cancelada
              NumeroLote = None
              NumeroProtocolo = None
              NumeroNota = Some numeroNota
              DataEmissao = Some dataEmissao
              ErrosComunicacao = None
              Rps = fromRpsDomain rps |> Some
              CodigoCancelamento = Some codigoCancelamento
              DataCancelamento = Some dataCancelamento }


let fromDomain (notaFiscal: NotaFiscalServico) : NotaFiscalServicoDto =
    { Id = notaFiscal.Id
      Tomador = TomadorDto.fromTomadorDomain notaFiscal.Tomador
      Servico = ServicoDto.fromServicoDomain notaFiscal.Servico
      Status = StatusDto.fromStatusDomain notaFiscal.Status }