module NotaFiscal.Domain.Dto.Errors

open System
open NotaFiscal.Domain.ApplicationErrors

type MissingDataError =
    | ContatoPessoaJuridicaIsRequired
    | EnderecoPessoaJuridicaIsRequired
    | ValoresServicoIsRequired
    | ServicoIsRequired
    | TomadorIsRequired

type NotaFiscalMapDomainErrors =
    | InvalidData of NotaFiscalErrors
    | MissingData of MissingDataError