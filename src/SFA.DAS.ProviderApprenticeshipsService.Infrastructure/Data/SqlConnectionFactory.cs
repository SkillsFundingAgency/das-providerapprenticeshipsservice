using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
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

        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        var useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);

        if (!useManagedIdentity)
        {
            logger.LogWarning("SqlConnectionFactory is not using managed identity.");
            return new SqlConnection(connectionString);
        }

        logger.LogWarning("SqlConnectionFactory is using managed identity.");
        var azureServiceTokenProvider = new AzureServiceTokenProvider();

        return new SqlConnection
        {
            ConnectionString = connectionString,
            AccessToken = await azureServiceTokenProvider.GetAccessTokenAsync(AzureResource)
        };
    }
}