namespace NotaFiscal.Data

open System.Text.Json
open System.Text.Json.Serialization
open MongoDB.Bson.Serialization
open NotaFiscal.Domain.Dto.NotaFiscalServicoDto

type FSharpCustomSerializaer =
    interface IBsonSerializer with
        member this.Deserialize(context: BsonDeserializationContext, args: BsonDeserializationArgs) =
            
            let serializationOpt =
                JsonFSharpOptions.Default().ToJsonSerializerOptions()

            let value = context.Reader.ReadString()
            let result = JsonSerializer.Deserialize value
            context.Reader.ReadEndDocument()
            result
        member this.Serialize(context: BsonSerializationContext, args: BsonSerializationArgs, value: obj) =
            let value = value :?> NotaFiscalServicoDto
            context.Writer.WriteString(serialize value)
            context.Writer.WriteEndDocument()
            ()

