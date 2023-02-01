﻿using Microsoft.Azure.Services.AppAuthentication;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public abstract class BaseRepository<T>
    {        
        private static string AzureResource = "https://database.windows.net/";
        private readonly string _connectionString;
        public IConfiguration _configuration;
        private readonly ILogger<T> _logger;
        private readonly Policy _retryPolicy;        
        private static IList<int> _transientErrorNumbers = new List<int>
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
            _configuration = configuration;
            _retryPolicy = GetRetryPolicy();
        }

        protected async Task<T> WithConnection<T>(Func<SqlConnection, Task<T>> getData)
        {
            try
            {
                return await _retryPolicy.Execute(async () =>
                {
                    using (var connection = GetSqlConnecction(_connectionString))
                    {
                        await connection.OpenAsync();
                        return await getData(connection);
                    }
                });
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a timeout", ex);
            }
            catch (SqlException ex) when (_transientErrorNumbers.Contains(ex.Number))
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a transient SQL Exception. ErrorNumber {ex.Number}", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a non-transient SQL exception (error code {ex.Number})", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced an exception (not a SQL Exception)", ex);
            }
        }

        protected async Task<T> WithTransaction<T>(Func<IDbConnection, IDbTransaction, Task<T>> getData)
        {
            try
            {

                return await _retryPolicy.Execute(async () =>
                {
                    using (var connection = GetSqlConnecction(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var trans = connection.BeginTransaction())
                        {
                            var data = await getData(connection, trans);
                            trans.Commit();
                            return data;
                        }
                    }
                });
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (SqlException ex) when (_transientErrorNumbers.Contains(ex.Number))
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a transient SQL Exception. ErrorNumber {ex.Number}", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a non-transient SQL exception (error code {ex.Number})", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced an exception (not a SQL Exception)", ex);
            }
        }

        protected async Task WithTransaction(Func<IDbConnection, IDbTransaction, Task> command)
        {
            try
            {
                await _retryPolicy.Execute(async () =>
                {
                    using (var connection = GetSqlConnecction(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var trans = connection.BeginTransaction())
                        {
                            await command(connection, trans);
                            trans.Commit();
                        }
                    }
                });
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (SqlException ex) when (_transientErrorNumbers.Contains(ex.Number))
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a transient SQL Exception. ErrorNumber {ex.Number}", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a non-transient SQL exception (error code {ex.Number})", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
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
                        _logger.LogWarning($"SqlException ({exception.Message}). Retrying...attempt {retryCount})");
                    }
                );
        }

        private SqlConnection GetSqlConnecction(string connectionString)
        {
            bool isLocal = _configuration["EnvironmentName"]?.Equals("LOCAL") ?? false;
            if (isLocal)
            {
                return new SqlConnection(connectionString);
            }
            else
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result;
                return new SqlConnection
                {
                    ConnectionString = connectionString,
                    AccessToken = accessToken,
                };
            }
        }
    }
}