using System;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public static class SqlConnectionFactory
{
    private const string AzureResource = "https://database.windows.net/";

    public static async Task<SqlConnection> GetConnectionAsync(string connectionString, ILogger logger)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        var useManagedIdentity = ConnectionUsesManagedIdentity(connectionString);

        var connection = new SqlConnection(connectionString);

        if (!useManagedIdentity)
        {
            logger.LogWarning("{TypeName} is not using managed identity.", nameof(SqlConnectionFactory));
            
            return connection;
        }

        logger.LogWarning("{TypeName} is using managed identity.", nameof(SqlConnectionFactory));

        connection.AccessToken = await new AzureServiceTokenProvider().GetAccessTokenAsync(AzureResource);

        return connection;
    }

    private static bool ConnectionUsesManagedIdentity(string connectionString)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        return !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);
    }
}