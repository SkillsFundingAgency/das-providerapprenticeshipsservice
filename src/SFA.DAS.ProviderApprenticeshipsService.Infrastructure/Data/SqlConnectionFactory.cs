using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public static class SqlConnectionFactory
{
    private const string AzureResource = "https://database.windows.net/";

    public static async Task<SqlConnection> GetConnectionAsync(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        var useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);

        if (!useManagedIdentity)
        {
            return new SqlConnection(connectionString);
        }

        var azureServiceTokenProvider = new AzureServiceTokenProvider();

        return new SqlConnection
        {
            ConnectionString = connectionString,
            AccessToken = await azureServiceTokenProvider.GetAccessTokenAsync(AzureResource)
        };
    }
}