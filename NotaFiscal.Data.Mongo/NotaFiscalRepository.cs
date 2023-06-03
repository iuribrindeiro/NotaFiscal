using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static NotaFiscal.Domain.Dto;
using static Rop;

namespace NotaFiscal.Data.Mongo;

public record struct FalhaComunicarBancoDados; 


public interface INotaFiscalRepository
{
    Task<OperationResult<FSharpOption<NotaFiscalServicoDto>, FalhaComunicarBancoDados>> FindByIdAsync(Guid id);
}

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly ILogger<NotaFiscalRepository> _logger;
    private readonly IMongoCollection<NotaFiscalServicoDto> _notasFiscaisCollection;

    public NotaFiscalRepository(IOptions<MongoDbOptions> mongoDbOptions, ILogger<NotaFiscalRepository> logger)
    {
        _logger = logger;
        var options = mongoDbOptions.Value;
        _notasFiscaisCollection = new MongoClient(options.ConnectionString)
            .GetDatabase(options.DatabaseName)
            .GetCollection<NotaFiscalServicoDto>(nameof(NotaFiscalServicoDto));
    }

    public async Task<OperationResult<FSharpOption<NotaFiscalServicoDto>, FalhaComunicarBancoDados>> FindByIdAsync(Guid id)
    {
        try
        {
            var notaFiscal = await _notasFiscaisCollection.AsQueryable().FirstOrDefaultAsync(e => e.Id == id);
            if (notaFiscal is null)
                return succeed<FSharpOption<NotaFiscalServicoDto>, FalhaComunicarBancoDados>(FSharpOption<NotaFiscalServicoDto>.None);
            
            notaFiscal.Tomador.Cnpj.
            return succeed<FSharpOption<NotaFiscalServicoDto>, FalhaComunicarBancoDados>(notaFiscal);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Falha ao comunicar com o banco de dados");
            return fail<FalhaComunicarBancoDados, FSharpOption<NotaFiscalServicoDto>>(new FalhaComunicarBancoDados());
        }
    }
}

public record MongoDbOptions
{
    public string ConnectionString { get; init; }
    public string DatabaseName { get; init; }
}
