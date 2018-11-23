using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Abstractions.Extensions;
using Plato.Internal.Data.Providers;

namespace Plato.Internal.Data
{
    public class DbContext : IDbContext, IDisposable
    {
        #region "Private Variables"

        private IDataProvider _provider;
        private readonly ILogger<DbContext> _logger;

        #endregion

        #region "Public Properties"

        public DbContextOptions Configuration { get; set; }

        #endregion

        #region "Events"
        
        public event DbEventHandlers.DbExceptionEventHandler OnException;
        
        #endregion

        #region "Dispose"

        public void Dispose()
        {
            _provider?.Dispose();
        }

        #endregion

        #region "Constructos"

        public DbContext(
            IOptions<DbContextOptions> dbContextOptions,
            ILogger<DbContext> logger)
        {
            _logger = logger;
            ConfigureInternal(dbContextOptions.Value);
        }

        public DbContext(Action<DbContextOptions> options)
        {
            var cfg = new DbContextOptions();
            options(cfg);
            Configure(options);
        }

        public void Configure(Action<DbContextOptions> options)
        {
            var cfg = new DbContextOptions();
            options(cfg);
            ConfigureInternal(cfg);
        }
        
        private void ConfigureInternal(DbContextOptions cfg)
        {
            var providerFactory = new DataProviderFactory(cfg);
            _provider = providerFactory.Provider;
            if (_provider != null)
            {
                // handle exceptions within the provider
                if (this.OnException == null)
                {
                    _provider.OnException += (sender, args) =>
                    {
                        HandleException(args.Exception);
                    };
                }
                else
                {
                    // dbContext has a explict exception handler
                    _provider.OnException += this.OnException;
                }
                    
            }
            Configuration = cfg;
        }

        #endregion

        #region "Implementation"

        public async Task<DbDataReader> ExecuteReaderAsync(
            CommandType commandType,
            string sql,
            params object[] commandParams)
        {
            if (_provider == null)
                return null;
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, commandParams);
            return await _provider.ExecuteReaderAsync(sql, commandParams);
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
          
            // Build a collection of output params and there index
            var outputParams = new Dictionary<int, IDbDataParameter>(); ;
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    if (args[i].GetType() == typeof(DbDataParameter))
                    {
                        if (((DbDataParameter)args[i]).Direction == ParameterDirection.Output)
                        {
                            outputParams.Add(i, ((IDbDataParameter)args[i]));
                        }
                    }
                }
            }

            var sb = new StringBuilder();
            if (outputParams.Count > 0)
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

                if (outputParams.ContainsKey(i))
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

            if (outputParams.Count > 0)
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
            _provider?.Dispose();
            throw ex;
        }

        #endregion



    }


}