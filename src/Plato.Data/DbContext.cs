using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Plato.Data
{
    public class DbContext : IDbContextt
    {

        #region "Private Variables"

        private string _connectionString;    
        private IDbConnection _connection;
        private IDataProvider _provider;

        #endregion

        #region "Constructos"

        public DbContext()
        {
            _connectionString =  "";
        }

        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
            _provider = ProviderFactory.GetProvider(_connectionString);
        }
        public DbContext(string connectionString, IDataProvider provider)
        {
            _connectionString = connectionString;
            _provider = provider;
        }

        #endregion

        #region "Implementation"

        public IDataReader ExecuteReader(CommandType commandType, string sql, params object[] commandParams)
        {

            if (commandType == CommandType.StoredProcedure)            
                sql = GenerateExecuteStoredProcedureSql(sql, commandParams);            
            return _provider.ExecuteReader(sql, commandParams);
        }
        
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
            //_database.Dispose();
        }


        #endregion


    }
}
