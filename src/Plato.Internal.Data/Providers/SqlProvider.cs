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


        //private SqlConnection _connection;

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

        //public async Task OpenAsync()
        //{

        
        //    _connection = new SqlConnection(_connectionString);

        //    if (_logger.IsEnabled(LogLevel.Information))
        //    {
        //        _logger.LogInformation("Creating SqlConnection object");
        //    }

        //    if (_connection.State != ConnectionState.Open)
        //    {
                
        //        if (_logger.IsEnabled(LogLevel.Information))
        //        {
        //            _logger.LogInformation("Opening database connection");
        //        }

        //        await _connection.OpenAsync();

        //        if (_logger.IsEnabled(LogLevel.Information))
        //        {
        //            _logger.LogInformation("Opened database connection");
        //        }
        //    }
            
        //}

        //public void Close()
        //{

        //    if (_logger.IsEnabled(LogLevel.Information))
        //    {
        //        _logger.LogInformation("Closing database connection");
        //    }

        //    _connection?.Dispose();
            
        //    if (_logger.IsEnabled(LogLevel.Information))
        //    {
        //        _logger.LogInformation("Closed database connection");
        //    }
        //}

        public async Task<T> ExecuteReaderAsync<T>(string sql, Func<DbDataReader, Task<T>> populate,
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
                    using (var command = CreateCommand(conn, sql, args))
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
                    using (var cmd = CreateCommand(conn, sql, args))
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
                    using (var cmd = CreateCommand(conn, sql, args))
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
        
        SqlCommand CreateCommand(
            SqlConnection connection,
            string sql, 
            params object[] args)
        {

            // Create the command and add parameters
            var cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
          
            foreach (var item in args)
            {
                AddParam(cmd, item);                
            }

            if (!string.IsNullOrEmpty(sql))
                DoPreExecute(cmd);

            return cmd;
        }
        

        void DoPreExecute(IDbCommand cmd)
        {
            if (CommandTimeout != 0)
            {
                cmd.CommandTimeout = CommandTimeout;
            }
        }

        void AddParam(IDbCommand cmd, object item)
        {
            
            var p = cmd.CreateParameter();
            p.ParameterName = $"@{cmd.Parameters.Count}";
            
            if (item == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                var t = item.GetType();
                if (t == typeof(Guid))
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                }
                else if (t == typeof(byte[]))
                {
                    p.Value = item;
                    p.DbType = DbType.Binary;            
                }
                else if (t == typeof(string))
                {
                    p.Size = Math.Max(((string) item).Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = item;
                }
                else if (t == typeof(bool))
                {
                    p.Value = ((bool)item) ? 1 : 0;
                    p.DbType = DbType.Boolean;
                }
                else if (t == typeof(int))
                {
                    p.Value = ((int)item);
                }
                else if (t == typeof(DateTime?))
                {
                    p.Value = ((DateTime) item);
                }
                else if (t == typeof(DbDataParameter))
                {
                    var dbParam = (IDbDataParameter) item;
                    p.ParameterName = dbParam.ParameterName;
                    p.Value = dbParam.Value ?? DBNull.Value;
                    p.DbType = dbParam.DbType;
                    p.Direction = dbParam.Direction;
                }
                else
                {
                    p.Value = item;
                }
            }

            cmd.Parameters.Add(p);
            
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
