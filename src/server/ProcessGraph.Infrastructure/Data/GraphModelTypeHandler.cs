using System.Data;
using System.Text;
using System.Text.Json;
using Dapper;
using ProcessGraph.Application.Models;

namespace ProcessGraph.Infrastructure.Data;

public class GraphModelTypeHandler : SqlMapper.TypeHandler<GraphModel>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public override void SetValue(IDbDataParameter parameter, GraphModel? value)
    {
        if (value is null)
        {
            parameter.Value = DBNull.Value;
            return;
        }

        var json = JsonSerializer.Serialize(value, JsonOptions);

        parameter.DbType = DbType.String;
        parameter.Value = json;
    }

    public override GraphModel? Parse(object value)
    {
        if (value is DBNull) return null;

        string json;

        switch (value)
        {
            case string s:
                json = s;
                break;
            case byte[] bytes:
                json = Encoding.UTF8.GetString(bytes);
                break;
            default:
                json = value.ToString() ?? string.Empty;
                break;
        }

        if (string.IsNullOrWhiteSpace(json)) return GraphModel.CreateEmpty();

        return JsonSerializer.Deserialize<GraphModel>(json, JsonOptions) ?? GraphModel.CreateEmpty();
    }
}