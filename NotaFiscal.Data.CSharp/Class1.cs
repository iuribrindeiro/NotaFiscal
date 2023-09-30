using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization;
using NotaFiscal.Domain.Dto;

namespace NotaFiscal.Data.CSharp;

public class Class1
{
    public static void Do()
    {
        BsonSerializer.RegisterSerializer()
    }   
}

public class CustomSerializer : IBsonSerializer, IBsonIdProvider, IBsonDocumentSerializer
{
    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) 
        => throw new NotImplementedException();

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        throw new NotImplementedException();
    }

    public Type ValueType { get; }
    public bool TryGetMemberSerializationInfo(string memberName, [UnscopedRef] out BsonSerializationInfo serializationInfo) 
        => throw new NotImplementedException();

    public bool GetDocumentId(object document, [UnscopedRef] out object id, [UnscopedRef] out Type idNominalType,
        [UnscopedRef] out IIdGenerator idGenerator) =>
        throw new NotImplementedException();

    public void SetDocumentId(object document, object id)
    {
        throw new NotImplementedException();
    }
}