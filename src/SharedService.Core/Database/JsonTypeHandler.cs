using System.Data;
using System.Text;
using System.Text.Json;
using Dapper;

namespace SharedService.Core.Database;

public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        parameter.Value = value is null
            ? DBNull.Value
            : JsonSerializer.Serialize(value, _options);

        parameter.DbType = DbType.String;
    }

    public override T Parse(object value)
    {
        if (value is DBNull)
            return default!;

        string json = value switch
        {
            string s => s,
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            ReadOnlyMemory<byte> memory => Encoding.UTF8.GetString(memory.Span),
            _ => value.ToString()!
        };

        if (string.IsNullOrWhiteSpace(json))
            return default!;

        return JsonSerializer.Deserialize<T>(json, _options)!;
    }

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };
}