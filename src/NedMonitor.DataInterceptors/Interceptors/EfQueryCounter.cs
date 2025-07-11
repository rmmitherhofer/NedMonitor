using Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace NedMonitor.DataInterceptors.Interceptors;

/// <summary>
/// Interceptor de comandos do EF Core para contar queries executadas em contexto assíncrono.
/// </summary>
public class EfQueryCounter : DbCommandInterceptor, IQueryCounter
{
    private static readonly AsyncLocal<int> _count = new();

    /// <summary>
    /// Reseta a contagem de queries para zero.
    /// </summary>
    public void Reset() => _count.Value = 0;

    /// <summary>
    /// Obtém a contagem atual de queries executadas no contexto.
    /// </summary>
    public int GetCount() => _count.Value;

    private void Increment() => _count.Value++;

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Increment();
        return base.ReaderExecuting(command, eventData, result);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        Increment();
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        Increment();
        return base.ScalarExecuting(command, eventData, result);
    }
}