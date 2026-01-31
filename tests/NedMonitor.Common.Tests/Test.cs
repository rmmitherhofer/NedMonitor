using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;


namespace NedMonitor.Common.Tests;

public abstract class Test(ITestOutputHelper output)
{
    protected readonly ITestOutputHelper _output = output;

    public void Print(IEnumerable<ValidationResult> validationResults, object? result = null)
    {
        if (!validationResults.Any())
        {
            Print(result);
            return;
        }

        _output.WriteLine($"Foram encontrados {validationResults.Count()} notificações nesta validação: " + Environment.NewLine);

        string detail = string.Empty;

        foreach (var error in validationResults)
            detail += error.ErrorMessage + Environment.NewLine;

        _output.WriteLine("são elas: " + Environment.NewLine + detail);
    }

    public void Print(ModelStateDictionary notifications, object? result = null)
    {
        if (!notifications.Any())
        {
            Print(result);
            return;
        }

        _output.WriteLine($"Foram encontrados {notifications.Count()} notificações nesta validação: " + Environment.NewLine);

        string detail = string.Empty;

        foreach (var notification in notifications)
            detail += $"{notification.Key} - {notification.Value.Errors.First().ErrorMessage}" + Environment.NewLine;

        _output.WriteLine("são elas: " + Environment.NewLine + detail);
    }

    public void Print(string message, object? result = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            Print(result);
            return;
        }

        _output.WriteLine($"Foi encontrada 1 notificação nesta validação: " + Environment.NewLine);

        _output.WriteLine("são elas: " + Environment.NewLine + message);
    }

    public void Print<TException>(TException exception, object? result = null) where TException : Exception
    {
        if (exception is null)
        {
            Print(result);
            return;
        }

        _output.WriteLine($"Foi encontrada 1 exceção nesta validação: " + Environment.NewLine);

        _output.WriteLine("são elas: " + Environment.NewLine + $"{typeof(TException).Name} - {exception.Message}");
    }

    public void Print(object? result = null)
    {
        if (result is not null)
            _output.WriteLine($"Teste em {result} validado com sucesso" + Environment.NewLine); 
    }
}
