using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Data.Abstractions;

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
  
        //public async Task<T> ExecuteReaderAsync<T>(CommandType commandType, string sql, Func<DbDataReader, Task<T>> populate, params object[] args) where T : class
        //{
        //    if (_provider == null)
        //        return null;
        //    if (commandType == CommandType.StoredProcedure)
        //        sql = DbParameterHelper.CreateExecuteStoredProcedureSql(GetProcedureName(sql), args);
        //    return await _provider.ExecuteReaderAsync<T>(sql, populate, args);
        //}
        
        //public async Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] args)
        //{
        //    if (_provider == null)
        //        return default(T);
        //    if (commandType == CommandType.StoredProcedure)
        //        sql = DbParameterHelper.CreateScalarStoredProcedureSql(GetProcedureName(sql), args);
        //    return await _provider.ExecuteScalarAsync<T>(sql, args);
        //}

        //public async Task<T> ExecuteNonQueryAsync<T>(CommandType commandType, string sql, params object[] args)
        //{
        //    if (_provider == null)
        //        return default(T);
        //    if (commandType == CommandType.StoredProcedure)
        //        sql = DbParameterHelper.CreateExecuteStoredProcedureSql(GetProcedureName(sql), args);
        //    return await _provider.ExecuteNonQueryAsync<T>(sql, args);
        //}

        // --- Testing

        public async Task<T> ExecuteReaderAsync2<T>(CommandType commandType, string commandText, Func<DbDataReader, Task<T>> populate, DbParam[] dbParams = null) where T : class
        {
            if (_provider == null)
                return null;
            if (commandType == CommandType.StoredProcedure)
                commandText = GetProcedureName(commandText);
                //if (commandType == CommandType.StoredProcedure)
                //    sql = DbParameterHelper.CreateDbParamsScalarStoredProcedureSql(GetProcedureName(sql), dbParams);
            return await _provider.ExecuteReaderAsync2<T>(commandType, commandText, populate, dbParams);
        }

        public async Task<T> ExecuteScalarAsync2<T>(CommandType commandType, string commandText, DbParam[] dbParams)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                commandText = GetProcedureName(commandText);
            ////if (commandType == CommandType.StoredProcedure)
            ////    sql = DbParameterHelper.CreateDbParamsScalarStoredProcedureSql(GetProcedureName(sql), dbParams);
            return await _provider.ExecuteScalarAsync2<T>(commandType, commandText, dbParams);
        }

        public async Task<T> ExecuteNonQueryAsync2<T>(CommandType commandType, string sql, DbParam[] dbParams)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                sql = GetProcedureName(sql);
            //if (commandType == CommandType.StoredProcedure)
            //    sql = DbParameterHelper.CreateDbParamsScalarStoredProcedureSql(GetProcedureName(sql), dbParams);
            return await _provider.ExecuteNonQueryAsync2<T>(commandType, sql, dbParams);
        }

        public void Dispose()
        {
            
        }
        
        private string GetProcedureName(string procedureName)
        {
            return !string.IsNullOrEmpty(Configuration.TablePrefix)
                ? Configuration.TablePrefix + procedureName
                : procedureName;
        }
        
    }

}