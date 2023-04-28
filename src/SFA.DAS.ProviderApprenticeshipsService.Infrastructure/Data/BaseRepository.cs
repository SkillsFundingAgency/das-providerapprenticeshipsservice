using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public abstract class BaseRepository<T>
{
    public IConfiguration Configuration { get; set; }

    private const string AzureResource = "https://database.windows.net/";
    private readonly string _connectionString;
    private readonly ILogger<T> _logger;
    private readonly Policy _retryPolicy;

    private readonly IList<int> _transientErrorNumbers = new List<int>
    {
        // https://docs.microsoft.com/en-us/azure/sql-database/sql-database-develop-error-messages
        // https://docs.microsoft.com/en-us/azure/sql-database/sql-database-connectivity-issues
        4060, 40197, 40501, 40613, 49918, 49919, 49920, 11001,
        -2, 20, 64, 233, 10053, 10054, 10060, 40143
    };

    protected BaseRepository(string connectionString, ILogger<T> logger, IConfiguration configuration)
    {
        _connectionString = connectionString;
        _logger = logger;
        Configuration = configuration;
        _retryPolicy = GetRetryPolicy();
    }

    protected async Task<TResult> WithConnection<TResult>(Func<SqlConnection, Task<TResult>> getData)
    {
        try
        {
            return await _retryPolicy.Execute(async () =>
            {
                await using var connection = await GetSqlConnectionAsync(_connectionString);
                await connection.OpenAsync();

                return await getData(connection);
            });
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException($"{GetType().FullName}.WithConnection() experienced a timeout", ex);
        }
        catch (SqlException ex) when (_transientErrorNumbers.Contains(ex.Number))
        {
            throw new InvalidOperationException($"{GetType().FullName}.WithConnection() experienced a transient SQL Exception. ErrorNumber {ex.Number}", ex);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"{GetType().FullName}.WithConnection() experienced a non-transient SQL exception (error code {ex.Number})", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{GetType().FullName}.WithConnection() experienced an exception (not a SQL Exception)", ex);
        }
    }

    protected async Task WithTransaction(Func<IDbConnection, IDbTransaction, Task> command)
    {
        try
        {
            await _retryPolicy.Execute(async () =>
            {
                await using var connection = await GetSqlConnectionAsync(_connectionString);
                await connection.OpenAsync();
                await using var trans = connection.BeginTransaction();
                await command(connection, trans);
                trans.Commit();
            });
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
        }
        catch (SqlException ex) when (_transientErrorNumbers.Contains(ex.Number))
        {
            throw new InvalidOperationException($"{GetType().FullName}.WithConnection() experienced a transient SQL Exception. ErrorNumber {ex.Number}", ex);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"{GetType().FullName}.WithConnection() experienced a non-transient SQL exception (error code {ex.Number})", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"{GetType().FullName}.WithConnection() experienced an exception (not a SQL Exception)", ex);
        }
    }

    private RetryPolicy GetRetryPolicy()
    {
        return Policy
            .Handle<SqlException>(ex => _transientErrorNumbers.Contains(ex.Number))
            .Or<TimeoutException>()
            .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("SqlException ({Message}). Retrying...attempt {RetryCount})", exception.Message, retryCount);
                }
            );
    }

    private async Task<SqlConnection> GetSqlConnectionAsync(string connectionString)
    {
        var isLocal = Configuration["EnvironmentName"]?.Equals("LOCAL") ?? false;
        if (isLocal)
        {
            return new SqlConnection(connectionString);
        }

        var tokenCredential = new DefaultAzureCredential();
        var accessToken = await tokenCredential.GetTokenAsync(
            new TokenRequestContext(scopes: new string[] { AzureResource + "/.default" }) { }
        );

        return new SqlConnection
        {
            ConnectionString = connectionString,
            AccessToken = accessToken.Token,
        };
    }
}