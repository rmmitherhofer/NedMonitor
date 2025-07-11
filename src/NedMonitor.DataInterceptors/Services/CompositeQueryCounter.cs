using NedMonitor.DataInterceptors.Interceptors;

namespace NedMonitor.DataInterceptors.Services;

/// <summary>
/// Serviço que combina contadores EF e Dapper para fornecer uma contagem única agregada.
/// </summary>
public class CompositeQueryCounter : IQueryCounter
{
    private readonly EfQueryCounter _efCounter;
    private readonly DapperQueryCounter _dapperCounter;

    public CompositeQueryCounter(EfQueryCounter efCounter, DapperQueryCounter dapperCounter)
    {
        _efCounter = efCounter;
        _dapperCounter = dapperCounter;
    }

    public void Reset()
    {
        _efCounter?.Reset();
        _dapperCounter?.Reset();
    }

    public int GetCount()
    {
        var efCount = _efCounter?.GetCount() ?? 0;
        var dapperCount = _dapperCounter?.GetCount() ?? 0;
        return efCount + dapperCount;
    }
}
