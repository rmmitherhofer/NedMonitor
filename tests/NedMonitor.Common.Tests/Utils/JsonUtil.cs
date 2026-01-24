using System.Text;
using Zypher.Json;

namespace NedMonitor.Common.Tests.Utils;

public static class JsonUtil
{
    public static string GetJsonFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;

        try
        {
            return File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}Files/Json/{path}.json", Encoding.GetEncoding("UTF-8"));
        }
        catch (Exception)
        {
            throw new FileNotFoundException($"Não foi possivel localizar o arquivo {AppDomain.CurrentDomain.BaseDirectory}Files/Json/{path}.json");
        }
    }
    public static TDto? GetDto<TDto>(string path)
    {
        var json = GetJsonFile(path);
        return JsonExtensions.Deserialize<TDto>(json);
    }
}