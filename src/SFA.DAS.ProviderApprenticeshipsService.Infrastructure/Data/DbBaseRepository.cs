using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class DbBaseRepository
    {
        private readonly string _connectionString;

        public DbBaseRepository(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
        }

        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(); // Asynchronously open a connection to the database
                    return await getData(connection);
                    // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a timeout", ex);
            }
            catch (SqlException ex)
            {
                if (ex.Number == -2) // SQL Server error number for connection timeout
                    throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);

                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL exception (error code {ex.Number})", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced an exception (not a SQL Exception)", ex);
            }
        }

        protected async Task WithTransaction(Func<IDbConnection, IDbTransaction, Task> getData)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(); // Asynchronously open a connection to the database
                    using (var trans = connection.BeginTransaction())
                    {
                        await getData(connection, trans);
                        trans.Commit();
                    }
                    // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a timeout", ex);
            }
            catch (SqlException ex)
            {
                if (ex.Number == -2) // SQL Server error number for connection timeout
                    throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);

                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL exception (error code {ex.Number})", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced an exception (not a SQL Exception)", ex);
            }
        }
    }
}
