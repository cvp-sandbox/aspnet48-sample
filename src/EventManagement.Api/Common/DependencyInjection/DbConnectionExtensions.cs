using Microsoft.Data.Sqlite;
using System.Data;

namespace EventManagement.Api.Common.DependencyInjection;

public static class DbConnectionExtensions
{
    public static IServiceCollection AddSqliteConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDbConnection>(sp =>
        {
            var dbPath = configuration["DatabasePath"];

            if (string.IsNullOrEmpty(dbPath))
            {
                throw new InvalidOperationException("DatabasePath configuration is missing or empty.");
            }
            string connectionString =
                configuration.GetConnectionString("DefaultConnection") ?? $"Data Source={dbPath};";

            var cs = connectionString.Replace("{DbFile}", dbPath);
            return new SqliteConnection(cs);
        });
        
        return services;
    }
}

