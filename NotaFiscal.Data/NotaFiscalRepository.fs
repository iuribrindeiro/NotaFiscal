namespace NotaFiscal.Data

open System
open MongoDB.Driver
open MongoDB.Driver.Linq
open NotaFiscal.Domain.Dto.NotaFiscalServicoDto
open NotaFiscal.Domain.Dto.Errors
open NotaFiscal.Domain.Rop

type DatabaseError =
    | DatabaseException of Exception
    | NotFound of string * string
    | FailedToDeserialize of NotaFiscalMapDomainErrors

module NotaFiscalRepository =
    let findNotaNotaById (collection: IMongoCollection<NotaFiscalServicoDto>) id =
        task {
            try
                let! nota = collection.AsQueryable().FirstOrDefaultAsync(fun e -> e.Id = id)
                let notaResult = match box nota with
                                    | null -> NotFound ("NotaFiscalServico", id.ToString()) |> fail
                                    | _ -> toDomain nota |> mapFailuresR FailedToDeserialize 
                return notaResult
            with ex -> return DatabaseException ex |> fail
        }
        
    let salvarNota (collection: IMongoCollection<NotaFiscalServicoDto>) nota =
        task {
            try
                do! fromDomain nota |> collection.InsertOneAsync
                return succeed ()
            with ex -> return DatabaseException ex |> fail 
        }