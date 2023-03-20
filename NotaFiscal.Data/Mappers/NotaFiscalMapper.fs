module NotaFiscal.Data.Mappers.NotaFiscalMapper

open NotaFiscal.Data.DbAutoGen
open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.Creators.ServicoCreator
open TomadorMapper
open NotaFiscalStatusMapper
open NotaFiscal.Domain.ValidationErrors

let createPrestadorFromDb cnpj inscricaoMunicipal =
    createPrestador <!> createCnpjPrestador cnpj
    <*> createInscricaoMunicipalPrestador inscricaoMunicipal


let createServicoFromDb (nota: dbo.NotaFiscalServico) =
    let valoresServicoResult =
        createValoresServico
            nota.ValoresServicos
            nota.ValoresDeducoes
            nota.ValoresPis
            nota.ValoresCofins
            nota.ValoresInss
            nota.ValoresIr
            nota.ValoresCsll
            nota.ValoresIss
            nota.ValoresIssDiscriminator
            nota.ValoresOutrasRetencoes
            nota.ValoresDescontoCondicionado
            nota.ValoresDescontoIncondicionado
            nota.ValoresAliquota


    createServico
        valoresServicoResult
        nota.ItemListaServico
        nota.CodigoTributacaoMunicipio
        nota.Discriminacao
        nota.MunicipioPrestacaoServico
        nota.CodigoCnae
        nota.ServicoNaturezaOperacaoId
        nota.ServicoRegimeEspecialTributacaoId
        nota.OptanteSimplesNacional
        nota.IncentivadorCultural

let createNotaFiscalWithStatus notaFiscal status = { notaFiscal with Status = status }

let createNotaFiscalFromDb
    (nota: dbo.NotaFiscalServico)
    (tomadorDb: dbo.Tomador option)
    (enderecoDb: dbo.Endereco option)
    (contato: dbo.Contato option)
    (errosComunicacao: dbo.ErroComunicacao list)
    =
    let servicoResult = (createServicoFromDb nota)

    let prestadorResult =
        createPrestadorFromDb nota.PrestadorCnpj nota.PrestadorInscricaoMunicipal

    let tomadorResult =
        createTomadorFromDb tomadorDb enderecoDb contato nota.TomadorDiscriminator

    let statusResult =
        createNotaStatusFromDb
            nota.Discriminator
            nota.RpsNumero
            nota.RpsSerie
            nota.TipoRpsId
            nota.DataEmissao
            nota.NumeroNota
            errosComunicacao
            nota.CodigoCancelamento
        |> mapFailures NotaFiscalStatusInvalido


    createNotaFiscalWithStatus
    <!> createNotaFiscalServico prestadorResult tomadorResult servicoResult
    <*> statusResult
