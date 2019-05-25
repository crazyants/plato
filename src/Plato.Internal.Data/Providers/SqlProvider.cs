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

        public async Task<T> ExecuteReaderAsync2<T>(CommandType commandType, string commandText, Func<DbDataReader, Task<T>> populate, DbParam[] dbParams = null) where T : class
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + commandText);
            }

            T output = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var command = CreateCommand(conn, commandType, commandText, dbParams))
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

        public async Task<T> ExecuteScalarAsync2<T>(CommandType commandType, string commandText, DbParam[] dbParams = null)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + commandText);
            }


            object output = null;
            using (var conn = new SqlConnection(_connectionString))
            {

                try
                {
                    await conn.OpenAsync();
                    using (var cmd = CreateCommand(conn, commandType, commandText, dbParams))
                    {

                        await cmd.ExecuteScalarAsync();

                        foreach (IDbDataParameter parameter in cmd.Parameters)
                        {
                            if (parameter.Direction == ParameterDirection.Output)
                            {
                                output = parameter.Value;
                                break;
                            }
                        }

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

        public async Task<T> ExecuteNonQueryAsync2<T>(CommandType commandType, string commandText, DbParam[] dbParams = null)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SQL to execute: " + commandText);
            }

            var output = default(T);
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var cmd = CreateCommand(conn, commandType, commandText, dbParams))
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
        
        SqlCommand CreateCommand(
            SqlConnection connection,
            CommandType commandType,
            string commandText,
            DbParam[] dbParams)
        {

            var cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;

            if (dbParams != null)
            {
                foreach (var parameter in dbParams)
                {
                    var p = CreateSqlParameter(parameter);
                    cmd.Parameters.Add(p);
                }
            }

            return cmd;

        }


        public IDbDataParameter CreateSqlParameter(DbParam dbParam)
        {

            //var p = cmd.CreateParameter();

            var p = new SqlParameter(); ;
            p.ParameterName = $"@{dbParam.ParameterName}";
            p.Value = dbParam.Value;
            p.Direction = dbParam.Direction;
            p.DbType = dbParam.DbType;

            if (dbParam.DbType == DbType.String || dbParam.DbType == DbType.AnsiString)
            {
                if (dbParam.Size > 0)
                {
                    p.Size = dbParam.Size;
                }
                else
                {
                    if (dbParam.Value != null)
                    {
                        p.Size = Math.Max(((string)dbParam.Value).Length + 1,
                            4000); // Help query plan caching by using common size;
                    }
                }
            }
            else
            {
                if (dbParam.Size > 0)
                    p.Size = dbParam.Size;
            }

            return p;

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
