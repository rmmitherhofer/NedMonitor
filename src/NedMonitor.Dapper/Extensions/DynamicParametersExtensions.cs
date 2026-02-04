using System.Collections;
using System.Reflection;

namespace NedMonitor.Dapper.Extensions;

internal static class DynamicParametersExtensions
{
    public static IDictionary<string, object?> GetDeclaredParameters(object parametersInstance)
    {
        ArgumentNullException.ThrowIfNull(parametersInstance, nameof(parametersInstance));

        Dictionary<string, object?> result = [];

        IDictionary? paramDict = TryGetParametersDictionary(parametersInstance);

        if (paramDict is null) return result;

        foreach (DictionaryEntry entry in paramDict)
        {
            string name = entry.Key.ToString() ?? string.Empty;
            object? paramInfo = entry.Value;

            result[name] = GetParameterValue(paramInfo);
        }
        return result;
    }

    private static IDictionary? TryGetParametersDictionary(object parametersInstance)
    {
        var type = parametersInstance.GetType();

        var field = type.GetField("parameters", BindingFlags.Instance | BindingFlags.NonPublic)
                  ?? type.GetField("_parameters", BindingFlags.Instance | BindingFlags.NonPublic)
                  ?? type.GetField("Parameters", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (field != null)
        {
            var value = field.GetValue(parametersInstance);
            if (value is IDictionary dict) return dict;
        }

        var property = type.GetProperty("Parameters", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property != null)
        {
            var value = property.GetValue(parametersInstance);
            if (value is IDictionary dict) return dict;
        }

        foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (typeof(IDictionary).IsAssignableFrom(f.FieldType))
            {
                var value = f.GetValue(parametersInstance);
                if (value is IDictionary dict) return dict;
            }
        }
        return null;
    }

    private static object? GetParameterValue(object paramInfo)
    {
        if (paramInfo is null) return null;

        var valueProp = paramInfo.GetType().GetProperty("Value",
                       BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        if (valueProp != null) return valueProp.GetValue(paramInfo);

        var valueField = paramInfo.GetType().GetField("value",
                        BindingFlags.Instance | BindingFlags.NonPublic)
                      ?? paramInfo.GetType().GetField("_value",
                        BindingFlags.Instance | BindingFlags.NonPublic);
        if (valueField != null) return valueField.GetValue(paramInfo);

        return null;
    }
}

