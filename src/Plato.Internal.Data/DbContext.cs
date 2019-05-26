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
  
        public async Task<T> ExecuteReaderAsync2<T>(CommandType commandType, string commandText, Func<DbDataReader, Task<T>> populate, IDbDataParameter[] dbParams = null) where T : class
        {
            if (_provider == null)
                return null;
            if (commandType == CommandType.StoredProcedure)
                commandText = GetProcedureName(commandText);
            return await _provider.ExecuteReaderAsync2<T>(commandType, commandText, populate, dbParams);
        }

        public async Task<T> ExecuteScalarAsync2<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                commandText = GetProcedureName(commandText);
            return await _provider.ExecuteScalarAsync2<T>(commandType, commandText, dbParams);
        }

        public async Task<T> ExecuteNonQueryAsync2<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams)
        {
            if (_provider == null)
                return default(T);
            if (commandType == CommandType.StoredProcedure)
                commandText = GetProcedureName(commandText);
            return await _provider.ExecuteNonQueryAsync2<T>(commandType, commandText, dbParams);
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