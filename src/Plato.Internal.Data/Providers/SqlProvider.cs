using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Data.Providers
{
    
    public class SqlProvider : IDataProvider
    {
        
        private readonly ILogger<SqlProvider> _logger;
        private readonly string _connectionString;
    
        public SqlProvider(
            ILogger<SqlProvider> logger,
            IOptions<DbContextOptions> dbContextOptions)
        {
            _logger = logger;
            _connectionString = dbContextOptions.Value.ConnectionString;
        }
        
        public int CommandTimeout { get; set; }
        
        public async Task<T> ExecuteReaderAsync<T>(
            string sql,
            Func<DbDataReader, Task<T>> populate,
            params object[] args) where T : class
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + sql);
            }
            
            T output = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var command = DbParameterHelper.CreateSqlCommand(conn, sql, args))
                    {
                        using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            OnExecutedCommand(command);
                            output = await populate(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                finally
                {
                    conn.Close();
                }
            }
            
            return output;

        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, params object[] args)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + sql);
            }


            object output = null;
            using (var conn = new SqlConnection(_connectionString))
            {

                try
                {
                    await conn.OpenAsync();
                    using (var cmd = DbParameterHelper.CreateSqlCommand(conn, sql, args))
                    {
                        output = await cmd.ExecuteScalarAsync();
                        OnExecutedCommand(cmd);
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                finally
                {
                    conn.Close();
                }
            }

            if (output != null)
            {
                return (T)Convert.ChangeType(output, typeof(T));
            }

            return default(T);

        }

        public async Task<T> ExecuteNonQueryAsync<T>(string sql, params object[] args)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + sql);
            }
            
            var output = default(T);
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var cmd = DbParameterHelper.CreateSqlCommand(conn, sql, args))
                    {
                        var returnValue = await cmd.ExecuteNonQueryAsync();
                        OnExecutedCommand(cmd);
                        output = (T) Convert.ChangeType(returnValue, typeof(T));
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                finally
                {
                    conn.Close();
                }
            }

            return output;

        }

        // -------- Testing

        public async Task<T> ExecuteReaderAsync2<T>(string sql, Func<DbDataReader, Task<T>> populate, DbParam[] dbParams = null) where T : class
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + sql);
            }

            T output = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var command = DbParameterHelper.CreateDbParamsSqlCommand(conn, sql, dbParams))
                    {
                        using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            OnExecutedCommand(command);
                            output = await populate(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                finally
                {
                    conn.Close();
                }
            }

            return output;
        }

        public async Task<T> ExecuteScalarAsync2<T>(string sql, DbParam[] dbParams = null)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + sql);
            }


            object output = null;
            using (var conn = new SqlConnection(_connectionString))
            {

                try
                {
                    await conn.OpenAsync();
                    using (var cmd = DbParameterHelper.CreateDbParamsSqlCommand(conn, sql, dbParams))
                    {
                        output = await cmd.ExecuteScalarAsync();
                        OnExecutedCommand(cmd);
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                finally
                {
                    conn.Close();
                }
            }

            if (output != null)
            {
                return (T)Convert.ChangeType(output, typeof(T));
            }

            return default(T);


        }

        public async Task<T> ExecuteNonQueryAsync2<T>(string sql, DbParam[] dbParams = null)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + sql);
            }

            var output = default(T);
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var cmd = DbParameterHelper.CreateDbParamsSqlCommand(conn, sql, dbParams))
                    {
                        var returnValue = await cmd.ExecuteNonQueryAsync();
                        OnExecutedCommand(cmd);
                        output = (T)Convert.ChangeType(returnValue, typeof(T));
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                finally
                {
                    conn.Close();
                }
            }

            return output;

        }

        // mainly used to hook in and override behaviour

        public virtual void OnExecutedCommand(IDbCommand cmd) { }
                
        public virtual void HandleException(Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, ex.Message);
            }
            throw ex;
        }

    }

}
