[<Microsoft.FSharp.Core.AutoOpen>]
module NotaFiscal.Domain.Entities.Tomador

open NotaFiscal.Domain.Entities.TomadorEstrangeiro
open NotaFiscal.Domain.Entities.TomadorPessoaFisica
open NotaFiscal.Domain.Entities.TomadorPessoaJuridica

type Tomador =
    | PessoaFisica of TomadorPessoaFisica option
    | PessoaJuridica of TomadorPessoaJuridica
    | Estrangeiro of TomadorEstrangeiro
