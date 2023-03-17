module NotaFiscal.Data.NotaRepository

open FSharp.Data.Sql
open NotaFiscal.Domain.NotaFiscalServico

type sql =
    SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, "Server=192.168.0.41,1433; Database=NotaFiscalDb; User Id=iuri; Password=1495;encrypt=False">

let ctx = sql.GetDataContext()

let getNotasAguardandoEnvio =
    query {
        for nota in ctx.Dbo.NotaFiscalServico do
            where (nota.Discriminator = nameof (AguardandoEnvio))
            select nota
    }
