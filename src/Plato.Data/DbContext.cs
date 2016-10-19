using System;
using System.Linq;
using System.Data;
using System.Text;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Plato.Data
{
    public class DbContext : IDbContextt, IDisposable
    {

        #region "Private Variables"
            
        private IDataProvider _provider;

        #endregion

        #region "Constructos"

        public DbContext(
            IOptions<DbContextOptions> dbContextOptions)
            : this(dbContextOptions.Value.ConnectionString, dbContextOptions.Value.ProviderName)
        {

        }
        
        public DbContext(string connectionString, string providerName = "SqlClient")
        {

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
                 
            var providerFactory = new ProviderFactory(connectionString, providerName);

            _provider = providerFactory.Provider;
                       
            _provider.OnException += (object sender, DbExceptionEventArgs args) =>
            {
                throw args.Exception;
            };

        }
        
        #endregion

        #region "Implementation"

        public IDataReader ExecuteReader(CommandType commandType, string sql, params object[] commandParams)
        {
            if (commandType == CommandType.StoredProcedure)            
                sql = GenerateExecuteStoredProcedureSql(sql, commandParams);            
            return _provider.ExecuteReader(sql, commandParams);
        }

        public async Task<IDataReader> ExecuteReaderAsync(CommandType commandType, string sql,  params object[] commandParams)
        {
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, commandParams);
            return await _provider.ExecuteReaderAsync(sql, commandParams);
        }


        public T ExecuteScalar<T>(CommandType commandType, string sql, params object[] args)
        {
            if (commandType == CommandType.StoredProcedure)            
                sql = GenerateExecuteStoredProcedureSql(sql, args);            
            return _provider.ExecuteScalar<T>(sql, args);
        }

        public async Task<T> ExecuteScalarAsync<T>(CommandType commandType, string sql, params object[] args)
        {
            if (commandType == CommandType.StoredProcedure)
                sql = GenerateExecuteStoredProcedureSql(sql, args);
            return await _provider.ExecuteScalarAsync<T>(sql, args);
        }

        public void Execute(CommandType commandType, string sql, params object[] args)
        {
            if (commandType == CommandType.StoredProcedure)            
                sql = GenerateExecuteStoredProcedureSql(sql, args);            
            _provider.Execute(sql, args);

        }

        #endregion

        #region "Private Methods"

        private static string GenerateExecuteStoredProcedureSql(string procedureName, params object[] args)
        {
            System.Text.StringBuilder sb = new StringBuilder(";EXEC ");
            sb.Append(procedureName);
            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(String.Format(" @{0}", i));
                if (i < args.Length - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region "Dispose"
        
        public void Dispose()
        {
            _provider.Dispose();
        }

      

        #endregion


    }
}
