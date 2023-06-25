namespace NotaFiscal.Data

open MongoDB.Driver
open MongoDB.Driver.Linq
open NotaFiscal.Domain.Dto.NotaFiscalServicoDto
open NotaFiscal.Domain.NotaFiscalServico
open NotaFiscal.Domain.Rop
open NotaFiscal.Domain.ApplicationErrors

module NotaFiscalRepository =
    let findNotaNotaById (collection: IMongoCollection<NotaFiscalServicoDto>) id =
        task {
            try
                let! nota = collection.AsQueryable().FirstOrDefaultAsync(fun e -> e.Id = id)
                
                return
                    match box nota with
                    | null -> NotFound (nameof NotaFiscalServico, id.ToString()) |> fail
                    | _ -> toDomain nota
                        |> mapFailuresR (FailedDeserializeFromDb >> InternalServerError)
            with ex -> return ex
                              |> (DatabaseException >> InternalServerError)
                              |> fail
        }
        
    let salvarNota (collection: IMongoCollection<NotaFiscalServicoDto>) nota =
        task {
            try
                do! fromDomain nota |> collection.InsertOneAsync
                return succeed ()
            with ex -> return ex
                              |> (DatabaseException >> InternalServerError)
                              |> fail 
        }