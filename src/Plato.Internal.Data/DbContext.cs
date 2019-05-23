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
    public class DbContext : IDbContext
    {
     
        public DbContextOptions Configuration { get; private set; }
        
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
  
        public async Task<T> ExecuteReaderAsync<T>(CommandType commandType, string sql, Func<DbDataReader, Task<T>> populate, params object[] args) where T : class
        {
            if (_provider == null)
                return null;
            if (commandType == CommandType.StoredProcedure)
                sql = DbParameterHelper.CreateExecuteStoredProcedureSql(GetProcedureName(sql), args);
            return await _provider.ExecuteReaderAsync<T>(sql, populate, args);
        }

        public async Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] args)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                sql = DbParameterHelper.CreateScalarStoredProcedureSql(GetProcedureName(sql), args);
            return await _provider.ExecuteScalarAsync<T>(sql, args);
        }

        public async Task<T> ExecuteNonQueryAsync<T>(CommandType commandType, string sql, params object[] args)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                sql = DbParameterHelper.CreateExecuteStoredProcedureSql(GetProcedureName(sql), args);
            return await _provider.ExecuteNonQueryAsync<T>(sql, args);
        }

        public void Dispose()
        {
            
        }
        
        //private string GenerateExecuteStoredProcedureSql(string procedureName, params object[] args)
        //{
        //    // Execute procedure 
        //    var sb = new StringBuilder("; EXEC ");
        //    sb.Append(GetProcedureName(procedureName));

        //    for (var i = 0; i < args.Length; i++)
        //    {
        //        sb.Append($" @{i}");
        //        if (i < args.Length - 1)
        //            sb.Append(",");
        //    }



        //    return sb.ToString();
        //}

     
        private string GetProcedureName(string procedureName)
        {
            return !string.IsNullOrEmpty(Configuration.TablePrefix)
                ? Configuration.TablePrefix + procedureName
                : procedureName;
        }
        
    }

}