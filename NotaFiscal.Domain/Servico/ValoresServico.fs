module NotaFiscal.Domain.ValoresServico

open NotaFiscal.Domain.NotaFiscalPrimitives
open NotaFiscal.Domain.DomainEvents

type Valores =
    { Servicos: Dinheiro
      Deducoes: MaybeDinheiro
      Pis: MaybeDinheiro
      Cofins: MaybeDinheiro
      Inss: MaybeDinheiro
      Ir: MaybeDinheiro
      Csll: MaybeDinheiro
      Iss: Iss
      OutrasRetencoes: MaybeDinheiro
      DescontoCondicionado: MaybeDinheiro
      DescontoIncondicionado: MaybeDinheiro
      Aliquota: MaybePercentage }

and Iss =
    | Retido of Dinheiro
    | NaoRetido of Dinheiro

let mapDinheiroErrors f err = mapFailuresR (fun _ -> f) err

let createValor valor =
    Dinheiro.create valor
    |> mapDinheiroErrors ValorServicoInvalido

let createValorDeducoes valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors ValorDeducoesInvalido

let createValorPis valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors ValorPisInvalido

let createValorCofins valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors ValorCofinsInvalido

let createValorInss valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors ValorInssInvalido

let createValorIr valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors ValorIrInvalido

let createValorCsll valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors ValorCsllInvalido

let createValorIssRetido valor =
    Retido <!> Dinheiro.create valor
    |> mapDinheiroErrors IssRetidoInvalido
    |> mapFailuresR IssInvalido

let createValorIssNaoRetido valor =
    NaoRetido <!> Dinheiro.create valor
    |> mapDinheiroErrors IssNaoRetidoInvalido
    |> mapFailuresR IssInvalido

let createValorOutrasRetencoes valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors OutrasRetencoesInvalido

let createValorDescontoIncondicionado valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors DescontoIncondicionadoInvalido

let createValorDescontoCondicionado valor =
    Dinheiro.createOptional valor
    |> mapDinheiroErrors DescontoCondicionadoInvalido

let createAliquota valor =
    Percentage.createOptional valor
    |> mapDinheiroErrors AliquotaInvalida

let createValorIss iss retido =
    match retido with
    | true -> createValorIssRetido iss
    | _ -> createValorIssNaoRetido iss


let createValoresServico
    valorServicos
    deducoes
    pis
    cofins
    inss
    ir
    csll
    valorIss
    issRetido
    outrasRetencoes
    descontoCondicionado
    descontoIncondicionado
    aliquota
    =
    let createValoresServico'
        servicos
        deducoes
        pis
        cofins
        inss
        ir
        csll
        outrasRetencoes
        iss
        aliquota
        descontoIncondicionado
        descontoCondicionado
        : Valores
        =
        { Servicos = servicos
          Deducoes = deducoes
          Pis = pis
          Cofins = cofins
          Inss = inss
          Ir = ir
          Csll = csll
          OutrasRetencoes = outrasRetencoes
          Iss = iss
          Aliquota = aliquota
          DescontoIncondicionado = descontoIncondicionado
          DescontoCondicionado = descontoCondicionado }


    createValoresServico'
    <!> createValor valorServicos
    <*> createValorDeducoes deducoes
    <*> createValorPis pis
    <*> createValorCofins cofins
    <*> createValorInss inss
    <*> createValorIr ir
    <*> createValorCsll csll
    <*> createValorOutrasRetencoes outrasRetencoes
    <*> createValorIss valorIss issRetido
    <*> createAliquota aliquota
    <*> createValorDescontoIncondicionado descontoIncondicionado
    <*> createValorDescontoCondicionado descontoCondicionado
    |> mapFailuresR (ValoresInvalidos >> ServicoInvalido)