module NotaFiscal.Data.NotaRepository

open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.Creators.TomadorCreator
open SqlHydra.Query
open Microsoft.Data.SqlClient
open NotaFiscal.Data.DbAutoGen
open NotaFiscal.Data.Mappers.NotaFiscalMapper

[<Literal>]
let connectionStringName = "NotaFiscal_ConnectionString"

let openContext () =
    let compiler = SqlKata.Compilers.SqlServerCompiler()


    let conn =
        new SqlConnection(System.Environment.GetEnvironmentVariable(connectionStringName))

    conn.Open()
    new QueryContext(conn, compiler)


let mapNotasFiscaisResults
    (results:
        (dbo.NotaFiscalServico *
        dbo.Tomador option *
        dbo.Endereco option *
        dbo.Contato option *
        dbo.ErroComunicacao option) seq Async)
    =
    async {
        let! notas = results

        return
            notas
            |> Seq.groupBy (fun (nota, _, _, _, _) -> nota.Id)
            |> Seq.map (fun (_, groupedResults) ->
                let (nota, tomador, endereco, contato, _) = groupedResults |> Seq.head

                let errosComunicacao =
                    groupedResults
                    |> Seq.map (fun (_, _, _, _, errosComunicacao) -> errosComunicacao)
                    |> Seq.toList
                    |> mapListOptionToList

                createNotaFiscalFromDb nota tomador endereco contato errosComunicacao)
    }


let private getNotasByStatusAsync' (context) (status) =
    selectAsync HydraReader.Read context {
        for nota in dbo.NotaFiscalServico do
            leftJoin t in dbo.Tomador on (nota.TomadorId.Value = t.Value.Id)
            leftJoin e in dbo.Endereco on (t.Value.EnderecoId.Value = e.Value.Id)
            leftJoin c in dbo.Contato on (t.Value.ContatoId.Value = c.Value.Id)
            leftJoin ec in dbo.ErroComunicacao on (nota.Id = ec.Value.NotaFiscalServicoId)
            where (nota.Discriminator = status)
            select (nota, t, e, c, ec)
    }
    |> mapNotasFiscaisResults

let getNotasByStatusAsync status =
    getNotasByStatusAsync' (Create openContext) status

let getNotasByStatusWithContextAsync status context =
    getNotasByStatusAsync' (Shared context) status
