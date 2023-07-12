using System;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public static class SqlConnectionFactory
{
    private const string AzureResource = "https://database.windows.net/";

    public static SqlConnection GetConnection(string connectionString)
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

        var azureServiceTokenProvider = new ChainedTokenCredential(
            new ManagedIdentityCredential(),
            new AzureCliCredential(),
            new VisualStudioCodeCredential(),
            new VisualStudioCredential());

        return new SqlConnection
        {
            ConnectionString = connectionString,
            AccessToken = azureServiceTokenProvider.GetToken(new TokenRequestContext(scopes: new[] { AzureResource })).Token
        };
    }
}