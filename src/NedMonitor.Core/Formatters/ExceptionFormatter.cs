using NedMonitor.Core.Settings;
using System.Text;

namespace NedMonitor.Core.Formatters;

/// <summary>
/// Formats exceptions into strings, optionally appending custom details from configured handlers.
/// Ensures exceptions are only logged once by marking them internally.
/// </summary>
public class ExceptionFormatter
{
    private const string ExceptionLoggedKey = "NedMonitor-ExceptionLogged";

    /// <summary>
    /// Formats the specified exception into a string, including any additional details provided by custom handlers.
    /// </summary>
    /// <param name="ex">The exception to format.</param>
    /// <returns>A formatted string representation of the exception.</returns>
    public string Format(Exception ex)
    {
        StringBuilder sb = new();

        FormatException(ex, sb);

        if (HandlerOptionsConfiguration.Options.Handlers.AppendExceptionDetails is not null)
        {
            string append = HandlerOptionsConfiguration.Options.Handlers.AppendExceptionDetails.Invoke(ex);
            if (!string.IsNullOrWhiteSpace(append))
            {
                sb.AppendLine();
                sb.AppendLine(append);
            }
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Recursively formats the exception and its base exception, avoiding duplicate logging.
    /// </summary>
    /// <param name="ex">The exception to format.</param>
    /// <param name="sb">The StringBuilder to append the formatted exception details.</param>
    /// <param name="header">An optional header to include before the exception details.</param>
    private void FormatException(Exception ex, StringBuilder sb, string? header = null)
    {
        string id = $"{ExceptionLoggedKey}-{Guid.NewGuid()}";

        bool alreadyLogged = ex.Data.Contains(id);

        if (alreadyLogged) return;

        ex.Data.Add(id, true);

        if (!string.IsNullOrEmpty(header)) sb.AppendLine(header);

        sb.AppendLine(ex.ToString());

        Exception? innerException = ex.InnerException;

        while (innerException is not null)
        {
            if (!innerException.Data.Contains(id))
                innerException.Data.Add(id, true);

            innerException = innerException.InnerException;
        }

        Exception baseException = ex.GetBaseException();

        if (baseException is not null)
            FormatException(baseException, sb, "Base Exception:");
    }
}
