using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Abstractions.Extensions;
using Plato.Internal.Data.Providers;

namespace Plato.Internal.Data
{
    public class DbContext : IDbContext, IDisposable
    {
     
        public DbContextOptions Configuration { get; set; }
        
        public event DbEventHandlers.DbExceptionEventHandler OnException;
        
        private readonly ILogger<DbContext> _logger;
        private readonly IDataProvider _provider;

        public DbContext(
            IOptions<DbContextOptions> dbContextOptions,
            IDataProvider provider,
            ILogger<DbContext> logger)
        {
            _provider = provider;
            _logger = logger;
            Configuration = dbContextOptions.Value;
        }
   
        public void Configure(Action<DbContextOptions> options)
        {
            var cfg = new DbContextOptions();
            options(cfg);
            Configuration = cfg;
        }
  
        #region "Implementation"
        
        public async Task<T> ExecuteReaderAsync<T>(CommandType commandType, string sql, Func<DbDataReader, Task<T>> populate, params object[] args) where T : class
        {
            if (_provider == null)
                return null;
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, args);
            return await _provider.ExecuteReaderAsync<T>(sql, populate, args);
        }

        public async Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] args)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateScalarStoredProcedureSql(sql, args);
            return await _provider.ExecuteScalarAsync<T>(sql, args);
        }

        public async Task<T> ExecuteNonQueryAsync<T>(CommandType commandType, string sql, params object[] args)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, args);
            return await _provider.ExecuteNonQueryAsync<T>(sql, args);
        }

        public void Dispose()
        {
            //_provider?.Dispose();
        }


        #endregion

        #region "Private Methods"

        private string GenerateExecuteStoredProcedureSql(string procedureName, params object[] args)
        {
            // Execute procedure 
            var sb = new StringBuilder("; EXEC ");
            sb.Append(GetProcedureName(procedureName));
            for (var i = 0; i < args.Length; i++)
            {
                sb.Append($" @{i}");
                if (i < args.Length - 1)
                    sb.Append(",");
            }
            return sb.ToString();
        }

        private string GenerateScalarStoredProcedureSql(string procedureName, params object[] args)
        {

            // Build a collection of output parameters and there index
            IDictionary<int, IDbDataParameter> outputParams = null; ;
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    if (args[i].GetType() == typeof(DbDataParameter))
                    {
                        if (((DbDataParameter)args[i]).Direction == ParameterDirection.Output)
                        {
                            if (outputParams == null)
                            {
                                outputParams = new Dictionary<int, IDbDataParameter>(); ;
                            }
                            outputParams.Add(i, ((IDbDataParameter)args[i]));
                        }
                    }
                }
            }

            var sb = new StringBuilder();
            if (outputParams != null)
            {
                foreach (var outputParam in outputParams)
                {
                    var name = !string.IsNullOrEmpty(outputParam.Value.ParameterName)
                        ? outputParam.Value.ParameterName
                        : outputParam.Key.ToString();
                    sb.Append($"DECLARE @{name}_out {outputParam.Value.DbTypeNormalized()};");
                }
            }
            
            sb.Append("EXEC ");
            sb.Append(GetProcedureName(procedureName));
            for (var i = 0; i < args.Length; i++)
            {

                if (outputParams?.ContainsKey(i) ?? false)
                {
                    var name = !string.IsNullOrEmpty(outputParams[i].ParameterName)
                        ? outputParams[i].ParameterName
                        : i.ToString();
                    sb.Append($" @{name}_out output");
                }
                else
                {
                    sb.Append($" @{i}");
                }

                if (i < args.Length - 1)
                    sb.Append(",");
            }

            sb.Append(";");

            // Return output parameters
            if (outputParams != null)
            {
                sb.Append("SELECT ");
                var i = 0;
                foreach (var outputParam in outputParams)
                {
                    var name = !string.IsNullOrEmpty(outputParam.Value.ParameterName)
                        ? outputParam.Value.ParameterName
                        : outputParam.Key.ToString();
                    sb.Append("@")
                        .Append(name)
                        .Append("_out");
                    if (i < outputParams.Count - 1)
                    {
                        sb.Append(",");
                    }
                    i++;
                }

                sb.Append(";");
            }

            return sb.ToString();
        }
        
        private string GetProcedureName(string procedureName)
        {
            return !string.IsNullOrEmpty(this.Configuration.TablePrefix)
                ? this.Configuration.TablePrefix + procedureName
                : procedureName;
        }

        public void HandleException(Exception ex)
        {
            //_provider?.Dispose();
            throw ex;
        }

        #endregion
        
    }

}