using DbUp;
using SimpleAuthAPI.Model;

namespace SimpleAuthAPI.Data;

public static class DbUpdate
{
    public static ResponseModel<bool> ApplyMigrations(IConfiguration configuration)
    {
        var resp = new ResponseModel<bool>();

        try
        {
            var connectionString =
                configuration.GetConnectionString("Database")
                ?? throw new InvalidOperationException(
                    "Connection string 'Database' not found.");

            EnsureDatabase.For.PostgresqlDatabase(connectionString);

            var upgrader =
                DeployChanges.To
                    .PostgresqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(
                        typeof(DbUpdate).Assembly,
                        script => script.StartsWith("SimpleAuthAPI.Data.Scripts"))
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                resp.Data = false;
                resp.ResponseType = ResponseType.ServerError;
                resp.Message = $"Error applying migrations: {result.Error}";
            }
            else
            {
                resp.ResponseType = ResponseType.Success;
                resp.Message = "Migrations applied successfully.";
                resp.Data = true;
            }
        }
        catch (Exception ex)
        {
            resp.Data = false;
            resp.ResponseType = ResponseType.ServerError;
            resp.Message = $"Unexpected error occurred: {ex.Message}";
        }

        return resp;
    }
}