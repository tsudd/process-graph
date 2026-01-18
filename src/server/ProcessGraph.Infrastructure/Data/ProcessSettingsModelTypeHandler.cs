using System.Data;
using Dapper;
using ProcessGraph.Application.Models;

namespace ProcessGraph.Infrastructure.Data;

public class ProcessSettingsModelTypeHandler : SqlMapper.TypeHandler<ProcessSettingsModel>
{
    public override void SetValue(IDbDataParameter parameter, ProcessSettingsModel? value)
    {
        throw new NotImplementedException();
    }

    public override ProcessSettingsModel? Parse(object value)
    {
        throw new NotImplementedException();
    }
}