module NotaFiscal.Data.NotaRepository

open FSharp.Data.Sql

type sql =
    SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, "Server=192.168.0.41,1433; Database=NotaFiscalDb; User Id=iuri; Password=1495;encrypt=False">
