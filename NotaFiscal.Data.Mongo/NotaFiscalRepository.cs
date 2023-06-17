using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static NotaFiscal.Domain.Dto;
using static Rop;

namespace NotaFiscal.Data.Mongo;

public interface INotaFiscalRepositoryErro {}

public record struct FalhaComunicarBancoDados : INotaFiscalRepositoryErro;
public record struct NotaNaoEncontrada : INotaFiscalRepositoryErro;


public interface INotaFiscalRepository
{
    Task<OperationResult<NotaFiscalServicoDto, INotaFiscalRepositoryErro>> FindByIdAsync(Guid id);
}

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly ILogger<NotaFiscalRepository> _logger;
    private readonly IMongoCollection<NotaFiscalServicoDto> _notasFiscaisCollection;

    public NotaFiscalRepository(IOptions<MongoDbOptions> mongoDbOptions, ILogger<NotaFiscalRepository> logger)
    {
        var lol = typeof(NotaFiscalRepository);
        _logger = logger;
        var options = mongoDbOptions.Value;
        _notasFiscaisCollection = new MongoClient(options.ConnectionString)
            .GetDatabase(options.DatabaseName)
            .GetCollection<NotaFiscalServicoDto>(nameof(NotaFiscalServicoDto));
    }

    public async Task<OperationResult<NotaFiscalServicoDto, INotaFiscalRepositoryErro>> FindByIdAsync(Guid id)
    {
        try
        {
            var notaFiscal = await _notasFiscaisCollection.AsQueryable().FirstOrDefaultAsync(e => e.Id == id);
            if (notaFiscal is null)
                return fail<INotaFiscalRepositoryErro, NotaFiscalServicoDto>(new NotaNaoEncontrada());
            
            return succeed<NotaFiscalServicoDto, INotaFiscalRepositoryErro>(notaFiscal);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Falha ao comunicar com o banco de dados");
            return fail<INotaFiscalRepositoryErro, NotaFiscalServicoDto>(new FalhaComunicarBancoDados());
        }
    }
}

public record MongoDbOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
}
