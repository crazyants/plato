using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Extensions;
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
        
        //SqlCommand CreateCommand(
        //    SqlConnection connection,
        //    string sql, 
        //    params object[] args)
        //{

        //    // Create the command and add parameters
        //    var cmd = connection.CreateCommand();
        //    cmd.Connection = connection;
        //    cmd.CommandText = sql;
            
        //    foreach (var value in args)
        //    {

        //        var valueType = value.GetType();
                
        //        // If we don't have any parameters yet
        //        // search for anonymous type first, we only
        //        // check for an anonymous type once for perf
        //        if (cmd.Parameters.Count == 0)
        //        {

        //            // If we have an anonymous type use the
        //            // property names from the anonymous type
        //            // as the parameter names for our query
        //            if (valueType.IsAnonymousType())
        //            {
        //                var properties = valueType.GetProperties();
        //                foreach (var property in properties)
        //                {
        //                    var propValue = property.GetValue(value, null);
        //                    CreateAndAddParam(
        //                        cmd,
        //                        $"@{property.Name}",
        //                        propValue,
        //                        propValue.GetType());
        //                }

        //                break;
        //            }
        //        }
                
        //        // use params object array
        //        CreateAndAddParam(
        //            cmd, 
        //            $"@{cmd.Parameters.Count}",
        //            value, 
        //            valueType);
                
        //    }

        //    if (!string.IsNullOrEmpty(sql))
        //        DoPreExecute(cmd);

        //    return cmd;
        //}
        

        //void DoPreExecute(IDbCommand cmd)
        //{
        //    if (CommandTimeout != 0)
        //    {
        //        cmd.CommandTimeout = CommandTimeout;
        //    }
        //}

        //void CreateAndAddParam(IDbCommand cmd, string name, object value, Type valueType)
        //{
            
        //    var p = cmd.CreateParameter();
        //    p.ParameterName = name;
            
        //    if (value == null)
        //    {
        //        p.Value = DBNull.Value;
        //    }
        //    else
        //    {

        //        if (valueType == typeof(Guid))
        //        {
        //            p.Value = value.ToString();
        //            p.DbType = DbType.String;
        //            p.Size = 40;
        //        }
        //        else if (valueType == typeof(byte[]))
        //        {
        //            p.Value = value;
        //            p.DbType = DbType.Binary;            
        //        }
        //        else if (valueType == typeof(string))
        //        {
        //            p.Size = Math.Max(((string) value).Length + 1, 4000); // Help query plan caching by using common size
        //            p.Value = value;
        //        }
        //        else if (valueType == typeof(bool))
        //        {
        //            p.Value = ((bool)value) ? 1 : 0;
        //            p.DbType = DbType.Boolean;
        //        }
        //        else if (valueType == typeof(int))
        //        {
        //            p.Value = ((int)value);
        //        }
        //        else if (valueType == typeof(DateTime?))
        //        {
        //            p.Value = ((DateTime) value);
        //        }
        //        else if (valueType == typeof(DbDataParameter))
        //        {
        //            var dbParam = (IDbDataParameter) value;
        //            p.ParameterName = dbParam.ParameterName;
        //            p.Value = dbParam.Value ?? DBNull.Value;
        //            p.DbType = dbParam.DbType;
        //            p.Direction = dbParam.Direction;
        //        }
        //        else
        //        {
        //            p.Value = value;
        //        }
        //    }

        //    cmd.Parameters.Add(p);
            
        //}
        
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
