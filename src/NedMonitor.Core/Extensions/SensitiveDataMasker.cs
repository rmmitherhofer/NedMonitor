using NedMonitor.Core.Settings;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NedMonitor.Core.Extensions;

/// <summary>
/// Masks sensitive data fields in objects by replacing values of specified keys with "***REDACTED***".
/// Works by serializing the object to JSON and recursively masking keys defined in configuration.
/// </summary>
public class SensitiveDataMasker
{
    private readonly HashSet<string> _sensitiveKeys;
    private readonly HashSet<string> _sensitivePatterns;
    private readonly string _maskValue;
    private readonly bool _enabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="SensitiveDataMasker"/> class.
    /// </summary>
    /// <param name="options">Options containing the list of sensitive keys to mask.</param>

    public SensitiveDataMasker(SensitiveDataMaskerSettings options)
    {
        _sensitiveKeys = new HashSet<string>(options.SensitiveKeys ?? [], StringComparer.OrdinalIgnoreCase);
        _sensitivePatterns = new HashSet<string>(options.SensitivePatterns ?? [], StringComparer.OrdinalIgnoreCase);

        _maskValue = options.MaskValue;
        _enabled = options.Enabled;
    }

    /// <summary>
    /// Masks sensitive data within the given object.
    /// </summary>
    /// <param name="input">The object to be masked.</param>
    /// <returns>A new object with sensitive fields masked, or the original object if masking fails.</returns>
    public object? Mask(object? input)
    {
        if (!_enabled || input == null)
            return input;

        return input switch
        {
            string str => MaskString(str),
            IDictionary<string, string> dict => Mask(dict),
            IDictionary<string, List<string>> listDict => Mask(listDict),
            IDictionary<string, object?> objDict => MaskObjectDictionary(objDict),
            IEnumerable<KeyValuePair<string, string>> enumerable => Mask(enumerable.ToDictionary()),
            _ => MaskComplexObject(input)
        };
    }


    public string? MaskString(string input)
    {
        if (!_enabled || string.IsNullOrEmpty(input)) return input;

        if (IsJson(input))
        {
            try
            {
                using var doc = JsonDocument.Parse(input);
                var masked = MaskElement(doc.RootElement);
                return JsonSerializer.Serialize(masked);
            }
            catch { }
        }

        var output = input;
        foreach (var pattern in _sensitivePatterns)
        {
            output = Regex.Replace(output, pattern, _maskValue,
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        return output;
    }

    /// <summary>
    /// Masks sensitive data in a dictionary of string keys and object values.
    /// </summary>
    public IDictionary<string, object?>? MaskObjectDictionary(IDictionary<string, object?> input)
    {
        if (!_enabled || input == null) return input;

        var output = new Dictionary<string, object?>(input.Count, StringComparer.OrdinalIgnoreCase);

        foreach (var item in input)
        {
            output[item.Key] = _sensitiveKeys.Contains(item.Key)
                ? _maskValue
                : Mask(item.Value);
        }

        return output;
    }
    private object? MaskComplexObject(object input)
    {
        try
        {
            var json = JsonSerializer.Serialize(input);
            using var doc = JsonDocument.Parse(json);
            return MaskElement(doc.RootElement);
        }
        catch
        {
            return input;
        }
    }

    private bool IsJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim();
        return (input.StartsWith("{") && input.EndsWith("}")) ||
               (input.StartsWith("[") && input.EndsWith("]"));
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
                    obj[prop.Name] = _sensitiveKeys.Contains(prop.Name)
                        ? _maskValue
                        : MaskElement(prop.Value);
                }
                return obj;

            case JsonValueKind.Array:
                return element.EnumerateArray().Select(MaskElement).ToList();

            case JsonValueKind.String:
                return MaskString(element.GetString()!);

            case JsonValueKind.Number:
                return element.TryGetInt64(out long l) ? l : element.GetDouble();

            case JsonValueKind.True: return true;
            case JsonValueKind.False: return false;
            case JsonValueKind.Null: return null;
            default: return null;
        }
    }

    /// <summary>
    /// Masks sensitive data in a dictionary of string keys and string values.
    /// </summary>
    public IDictionary<string, string>? Mask(IDictionary<string, string> data)
    {
        if (!_enabled || data is null) return data;

        var masked = new Dictionary<string, string>(data.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in data)
        {
            masked[kvp.Key] = _sensitiveKeys.Contains(kvp.Key) ? _maskValue : kvp.Value;
        }

        return masked;
    }

    /// <summary>
    /// Masks sensitive data in a dictionary of string keys and list of string values.
    /// </summary>
    public IDictionary<string, List<string>>? Mask(IDictionary<string, List<string>> data)
    {
        if (!_enabled || data == null) return data;

        var masked = new Dictionary<string, List<string>>(data.Count, StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in data)
        {
            masked[kvp.Key] = _sensitiveKeys.Contains(kvp.Key) ? [_maskValue] : [.. kvp.Value];
        }

        return masked;
    }

}