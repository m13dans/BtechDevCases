using Npgsql;
using System.Data;

namespace SimpleAuthAPI.Data;

public class ConnectionFactory(string connectionString)
{
    public IDbConnection Create() => new NpgsqlConnection(connectionString);
}
