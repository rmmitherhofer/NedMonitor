namespace NedMonitor.DataInterceptors.Interceptors;

/// <summary>
/// Contador de queries para Dapper, baseado em AsyncLocal para contexto de requisição.
/// </summary>
public class DapperQueryCounter : IQueryCounter
{
    private static readonly AsyncLocal<int> _count = new();

    /// <inheritdoc/>
    public void Reset() => _count.Value = 0;

    /// <inheritdoc/>
    public int GetCount() => _count.Value;

    /// <summary>
    /// Incrementa o contador de queries.
    /// Deve ser chamado pelo wrapper da conexão Dapper.
    /// </summary>
    public void Increment() => _count.Value++;
}
