using System.Data;
using ProcessGraph.Application.Abstractions.Data;

namespace ProcessGraph.Infrastructure.Data;

public sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new Npgsql.NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}