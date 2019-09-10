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
        
        public int CommandTimeout { get; set; } = 120;
        
        private readonly ILogger<SqlProvider> _logger;
        private readonly string _connectionString;
    
        public SqlProvider(
            ILogger<SqlProvider> logger,
            IOptions<DbContextOptions> dbContextOptions)
        {
            _logger = logger;
            _connectionString = dbContextOptions.Value.ConnectionString;
        }

        public async Task<T> ExecuteReaderAsync<T>(CommandType commandType, string commandText, Func<DbDataReader, Task<T>> populate, IDbDataParameter[] dbParams = null) where T : class
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
                            output = await populate(new CachedDbDataReader(reader));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log and rethrow the exception
                    HandleException(ex);
                }
                finally
                {
                    conn.Close();
                }
            }

            return output;
        }

        public async Task<T> ExecuteScalarAsync<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams = null)
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

                        var result = await cmd.ExecuteScalarAsync();
                        
                        // Use result if available
                        if (result != null)
                        {
                            output = result;
                        }

                        // Use output parameters if available
                        if (cmd.Parameters != null)
                        {
                            foreach (IDbDataParameter parameter in cmd.Parameters)
                            {
                                if (parameter.Direction == ParameterDirection.Output)
                                {
                                    output = parameter.Value;
                                    break;
                                }
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

        public async Task<T> ExecuteNonQueryAsync<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams = null)
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

                        //  returns the number of rows affected
                        var rowsEffected = await cmd.ExecuteNonQueryAsync();

                        // Use output parameters if available
                        if (cmd.Parameters != null)
                        {
                            foreach (IDbDataParameter parameter in cmd.Parameters)
                            {
                                if (parameter.Direction == ParameterDirection.Output)
                                {
                                    output = parameter.Value;
                                    break;
                                }
                            }
                        }

                        // If we don't have any output params to populate the value
                        // Return the standard number of rows effected by the query
                        if (output == null)
                        {
                            output = rowsEffected;
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
                output = (T)Convert.ChangeType(output, typeof(T));
            }

            return default(T);

        }
        
        // ---------------

        SqlCommand CreateCommand(
            SqlConnection connection,
            CommandType commandType,
            string commandText,
            IDbDataParameter[] dbParams)
        {

            var cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            cmd.CommandTimeout = CommandTimeout;

            if (dbParams != null)
            {
                foreach (var parameter in dbParams)
                {
                    cmd.Parameters.Add(CreateParameter(cmd, parameter));
                }
            }

            return cmd;

        }

        public IDbDataParameter CreateParameter(IDbCommand cmd, IDbDataParameter dbParam)
        {

            var p = cmd.CreateParameter();
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
            if (ex == null) return;
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, ex.Message);
            throw ex;
        }

    }

}
