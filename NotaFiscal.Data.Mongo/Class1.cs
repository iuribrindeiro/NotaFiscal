namespace NotaFiscal.Data.Mongo;

using static NotaFiscal.Domain.DTO;

public class Class1
{
    public Class1()
    {
        var lol = new NotaFiscalServicoDTO(Guid.NewGuid(), new TomadorDTO(Guid.NewGuid(), string.Empty));
    }
}
