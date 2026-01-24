using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;
using Zypher.Notifications.Handlers;
using Zypher.Notifications.Interfaces;
using Zypher.Notifications.Messages;

namespace NedMonitor.Common.Tests;

public abstract class Test
{
    protected readonly ITestOutputHelper _output;
    protected readonly INotificationHandler _notification;

    protected Test(ITestOutputHelper output)
    {
        _output = output;
        var logger = new Mock<ILogger<NotificationHandler>>();
        _notification = new NotificationHandler(new HttpContextAccessor(), logger.Object);
    }

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

    public void Print(IEnumerable<Notification> notifications, object? result = null)
    {
        if (!notifications.Any())
        {
            Print(result);
            return;
        }

        _output.WriteLine($"Foram encontrados {notifications.Count()} notificações nesta validação: " + Environment.NewLine);

        string detail = string.Empty;

        foreach (var notification in notifications)
            detail += $"{notification.Type} - {notification.Key} - {notification.Value}" + Environment.NewLine;

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

        if (_notification.HasNotifications())
        {
            _output.WriteLine("Não foram encontradas notificações nesta validação");
            return;
        }

        _output.WriteLine($"Fora encontradas {_notification.Get().Count()} notificações nesta validação: " + Environment.NewLine);

        string detail = string.Empty;

        foreach (var notification in _notification.Get())
            detail += notification.Value + Environment.NewLine;

        _notification.Clear();

        _output.WriteLine("são elas: " + Environment.NewLine + detail);
    }
}
