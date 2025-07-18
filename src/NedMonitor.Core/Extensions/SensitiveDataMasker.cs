using NedMonitor.Core.Settings;
using System.Text.Json;
using Zypher.Json;

namespace NedMonitor.Core.Extensions;

/// <summary>
/// Masks sensitive data fields in objects by replacing values of specified keys with "***REDACTED***".
/// Works by serializing the object to JSON and recursively masking keys defined in configuration.
/// </summary>
public class SensitiveDataMasker
{
    private readonly HashSet<string> _sensitiveKeys;
    private readonly string _maskValue;
    private readonly bool _enabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="SensitiveDataMasker"/> class.
    /// </summary>
    /// <param name="options">Options containing the list of sensitive keys to mask.</param>
    public SensitiveDataMasker(SensitiveDataMaskerSettings options)
    {
        _sensitiveKeys = new HashSet<string>(options.SensitiveKeys, StringComparer.OrdinalIgnoreCase);
        _maskValue = options.MaskValue;
        _enabled = options.Enabled;
    }

    /// <summary>
    /// Masks sensitive data within the given object.
    /// </summary>
    /// <param name="data">The object to be masked.</param>
    /// <returns>A new object with sensitive fields masked, or the original object if masking fails.</returns>
    public object? Mask(object? data)
    {
        if (data is null) return null;

        if (!_enabled) return data;

        try
        {
            var json = JsonExtensions.Serialize(data);
            using var doc = JsonDocument.Parse(json);
            return MaskElement(doc.RootElement);
        }
        catch
        {
            return data;
        }
    }

    /// <summary>
    /// Masks sensitive data in a dictionary of string keys and string values.
    /// </summary>
    public IDictionary<string, string> Mask(IDictionary<string, string> data)
    {
        if (!_enabled || data is null) return data;

        var masked = new Dictionary<string, string>();
        foreach (var kvp in data)
        {
            masked[kvp.Key] = _sensitiveKeys.Contains(kvp.Key) ? _maskValue : kvp.Value;
        }

        return masked;
    }

    /// <summary>
    /// Masks sensitive data in a dictionary of string keys and list of string values.
    /// </summary>
    public IDictionary<string, List<string>> Mask(IDictionary<string, List<string>> data)
    {
        if (!_enabled || data is null) return data;

        var masked = new Dictionary<string, List<string>>();
        foreach (var kvp in data)
        {
            if (_sensitiveKeys.Contains(kvp.Key))
            {
                masked[kvp.Key] = [_maskValue];
            }
            else
            {
                masked[kvp.Key] = kvp.Value;
            }
        }

        return masked;
    }

    /// <summary>
    /// Recursively processes a JSON element, masking sensitive keys.
    /// </summary>
    /// <param name="element">The JSON element to mask.</param>
    /// <returns>A masked representation of the JSON element.</returns>
    private object? MaskElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new Dictionary<string, object?>();
                foreach (var prop in element.EnumerateObject())
                {
                    var key = prop.Name;
                    if (_sensitiveKeys.Contains(key))
                    {
                        obj[key] = _maskValue;
                    }
                    else
                    {
                        obj[key] = MaskElement(prop.Value);
                    }
                }
                return obj;

            case JsonValueKind.Array:
                return element.EnumerateArray().Select(MaskElement).ToList();

            case JsonValueKind.String: return element.GetString();
            case JsonValueKind.Number:
                if (element.TryGetInt64(out long longVal))
                    return longVal;
                if (element.TryGetDouble(out double doubleVal))
                    return doubleVal;
                return element.GetRawText();

            case JsonValueKind.True: return true;
            case JsonValueKind.False: return false;
            case JsonValueKind.Null: return null;
            default: return null;
        }
    }

}