using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Extensions.Options;

namespace Plato.Data
{
    public class DbContext : IDbContextt, IDisposable
    {

        #region "Private Variables"

        private string _providerName;
        private string _connectionString;       
        private IDataProvider _provider;

        #endregion

        public DbContext(IOptions<DbContextOptions> dbContextOptions)
            : this(dbContextOptions.Value.ConnectionString, dbContextOptions.Value.ProviderName)
        {

        }

        #region "Constructos"

        public DbContext(string connectionString, string providerName = "SqlClient")
        {

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
            _providerName = providerName;
            _provider = ProviderFactory.GetProvider(_connectionString, _providerName);
            
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

        //public IDataReader ExecuteReader(CommandType commandType, string sql, List<DbParameter> commandParams)
        //{
        //    return _provider.ExecuteReader(sql, commandParams);
        //}

        public T ExecuteScalar<T>(CommandType commandType, string sql, params object[] args)
        {
            if (commandType == CommandType.StoredProcedure)            
                sql = GenerateExecuteStoredProcedureSql(sql, args);            
            return _provider.ExecuteScalar<T>(sql, args);
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
            for (int i = 0; i < args.Count(); i++)
            {
                sb.Append(String.Format(" @{0}", i));
                if (i < args.Count() - 1)
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
