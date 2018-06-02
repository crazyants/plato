using System;
using Plato.Abstractions.Data;

namespace Plato.Data.Providers
{
    public class DataProviderFactory
    {

        #region "Consts"

        // System.Data.SqlClient
        private const string SqlClient = "sqlclient";

        // System.Data.SqlClient
        private const string SqlLite = "sqllite";

        // System.Data.Odbc
        private const string Odbc = "odbc";    
                    
        // MySql.Data
        private const string MySql = "mysql";

        // System.Data.OracleClient
        private const string Oracle = "oracle";

        #endregion

        #region "Private Variables"

        private readonly DbContextOptions _cfg;
        private IDataProvider _provider;

        #endregion

        #region "Public Properties"
        
        public string ConnectionString => _cfg.ConnectionString;

        public IDataProvider Provider => _provider;

        #endregion

        #region "Constructor"
        
        public DataProviderFactory(DbContextOptions cfg)
        {
            _cfg = cfg;
            BuildDataProvider();
        }
        
        void BuildDataProvider()
        {

            switch (_cfg.DatabaseProvider?.ToLower())
            {
                case SqlClient:
                    {
                        _provider = new SqlProvider(_cfg.ConnectionString);
                        break;
                    }
            }
            
        }
        
        #endregion


    }
}
