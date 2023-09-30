module NotaFiscal.Domain.Dto.ServicoDto

open System
open NotaFiscal.Domain.Primitives
open NotaFiscal.Domain.Entities.Servico
open NotaFiscal.Domain.Entities.Servico.ValoresServico
open NotaFiscal.Domain.Rop
open NotaFiscal.Domain.DomainEvents

type ServicoDto =
    { Valores: ValoresDto
      ItemListaServico: string
      CodigoTributacaoMunicipio: string
      Discriminacao: string
      CodigoMunicipioPrestacao: string
      CodigoCnae: string
      NaturezaOperacao: int
      RegimeEspecialTributacao: int
      OptanteSimplesNacional: bool
      IncentivadorCultural: bool }
    
and ValoresDto =
    { Servicos: decimal
      Deducoes: decimal Nullable
      Pis: decimal Nullable
      Cofins: decimal Nullable
      Inss: decimal Nullable
      Ir: decimal Nullable
      Csll: decimal Nullable
      ValorIss: decimal
      IssRetido: bool
      OutrasRetencoes: decimal Nullable
      DescontoCondicionado: decimal Nullable
      DescontoIncondicionado: decimal Nullable
      Aliquota: double Nullable }
let mapValorIss (iss: Iss) =
        match iss with
        | Retido r -> Dinheiro.mapToValue r
        | NaoRetido r -> Dinheiro.mapToValue r
        
let isIssRetido (iss: Iss) : bool =
    match iss with
    | Retido _ -> true
    | NaoRetido _ -> false

let fromValoresDomain (valores: Valores) : ValoresDto =
    { Servicos = valores.Servicos |> Dinheiro.mapToValue
      Deducoes = Dinheiro.mapToValueOptional valores.Deducoes |> Option.withDefaultNullable
      Pis = Dinheiro.mapToValueOptional valores.Pis |> Option.withDefaultNullable
      Cofins = Dinheiro.mapToValueOptional valores.Cofins |> Option.withDefaultNullable
      Inss = Dinheiro.mapToValueOptional valores.Inss |> Option.withDefaultNullable
      Ir = Dinheiro.mapToValueOptional valores.Ir |> Option.withDefaultNullable
      Csll = Dinheiro.mapToValueOptional valores.Csll |> Option.withDefaultNullable
      ValorIss = mapValorIss valores.Iss
      IssRetido =  isIssRetido valores.Iss
      OutrasRetencoes = Dinheiro.mapToValueOptional valores.OutrasRetencoes |> Option.withDefaultNullable
      DescontoCondicionado = Dinheiro.mapToValueOptional valores.DescontoCondicionado |> Option.withDefaultNullable
      DescontoIncondicionado = Dinheiro.mapToValueOptional valores.DescontoIncondicionado |> Option.withDefaultNullable
      Aliquota = Percentage.mapToValueOptional valores.Aliquota |> Option.withDefaultNullable }

let fromServicoDomain (servico: Servico) : ServicoDto =
    { Valores = fromValoresDomain servico.Valores
      ItemListaServico = servico.ItemListaServico |> StrMax7.mapToValue
      CodigoTributacaoMunicipio = StrMax20.mapToValueOption servico.CodigoTributacaoMunicipio |> Option.defaultValue String.Empty
      Discriminacao = servico.Discriminacao |> StrMax2000.mapToValue
      CodigoMunicipioPrestacao =
        servico.CodigoMunicipioPrestacao |> StrMax7.mapToValue
      CodigoCnae = StrMax7.mapToValueOptional servico.CodigoCnae |> Option.defaultValue String.Empty
      NaturezaOperacao = servico.NaturezaOperacao |> NaturezaOperacao.mapValue
      RegimeEspecialTributacao =
        servico.RegimeEspecialTributacao
        |> RegimeEspecialTributacao.mapValue
      OptanteSimplesNacional = servico.OptanteSimplesNacional
      IncentivadorCultural = servico.IncentivadorCultural }

let toValoresDomain (valoresDto: ValoresDto) =
    createValoresServico
        <| valoresDto.Servicos
        <| Option.ofNullable valoresDto.Deducoes
        <| Option.ofNullable valoresDto.Pis
        <| Option.ofNullable valoresDto.Cofins
        <| Option.ofNullable valoresDto.Inss
        <| Option.ofNullable valoresDto.Ir
        <| Option.ofNullable valoresDto.Csll
        <| valoresDto.ValorIss
        <| valoresDto.IssRetido
        <| Option.ofNullable valoresDto.OutrasRetencoes
        <| Option.ofNullable valoresDto.DescontoCondicionado
        <| Option.ofNullable valoresDto.DescontoIncondicionado
        <| Option.ofNullable valoresDto.Aliquota
    

let toServicoDomain (servicoDto: ServicoDto) =
    match box servicoDto with
    | null -> ServicoIsRequired |> fail
    | _ ->
        createServico
            <| servicoDto.ItemListaServico
            <| Some servicoDto.CodigoTributacaoMunicipio
            <| servicoDto.Discriminacao
            <| servicoDto.CodigoMunicipioPrestacao
            <| Some servicoDto.CodigoCnae
            <| servicoDto.NaturezaOperacao
            <| servicoDto.RegimeEspecialTributacao
            <| servicoDto.OptanteSimplesNacional
            <| servicoDto.IncentivadorCultural
        <*> mapNullToR servicoDto.Valores toValoresDomain (ValoresIsRequired |> ServicoInvalido)
