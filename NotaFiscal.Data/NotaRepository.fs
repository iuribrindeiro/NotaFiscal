module NotaFiscal.Data.NotaRepository

open SqlHydra.Query
open Microsoft.Data.SqlClient
open NotaFiscal.Data.DbAutoGen
open NotaFiscal.Data.Mappers.NotaFiscalMapper
open NotaFiscal.Data.Mappers.TomadorMapper
open NotaFiscal.Domain.ApplicationErrors

[<Literal>]
let connectionStringName = "NotaFiscal_ConnectionString"

let openContext () =
    let compiler = SqlKata.Compilers.SqlServerCompiler()


    let conn =
        new SqlConnection(System.Environment.GetEnvironmentVariable(connectionStringName))

    conn.Open()
    new QueryContext(conn, compiler)


let getNotasByStatusAsync' context status =
    async {
        try
            let! results =
                selectAsync HydraReader.Read context {
                    for nota in dbo.NotaFiscalServico do
                        leftJoin t in dbo.Tomador on (nota.TomadorId.Value = t.Value.Id)
                        leftJoin e in dbo.Endereco on (t.Value.EnderecoId.Value = e.Value.Id)
                        leftJoin c in dbo.Contato on (t.Value.ContatoId.Value = c.Value.Id)
                        leftJoin ec in dbo.ErroComunicacao on (nota.Id = ec.Value.NotaFiscalServicoId)
                        where (nota.Discriminator = status)
                        select (nota, t, e, c, ec)
                }

            return results |> mapNotasFiscaisResults |> succeed

        with ex ->
            return fail ex
    }

let getNotasByStatusAsync status =
    getNotasByStatusAsync' (Create openContext) status

let getNotasByStatusWithContextAsync status context =
    getNotasByStatusAsync' (Shared context) status

let addNotaFiscalAsync notaFiscal =

    let notaDb, maybeContatoDb, maybeEnderecoDb, maybeTomadorDb, _ =
        toNotaFiscalDb notaFiscal

    let ctx = openContext ()
    ctx.BeginTransaction()

    async {
        try

            let! _ = maybeAddContatoAsync ctx maybeContatoDb
            let! _ = maybeAddEnderecoAsync ctx maybeEnderecoDb
            let! _ = maybeAddTomadorAsync ctx maybeTomadorDb

            let! _ =
                insertAsync (Shared ctx) {
                    for _ in dbo.NotaFiscalServico do
                        entity notaDb
                }

            ctx.CommitTransaction()
            return succeed notaFiscal
        with ex ->
            ctx.RollbackTransaction()
            return FailToSaveNotaDb ex |> fail
    }
