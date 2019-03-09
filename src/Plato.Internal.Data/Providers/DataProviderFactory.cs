using System;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Data.Providers
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
   
        #endregion

        #region "Public Properties"
        
        public string ConnectionString => _cfg.ConnectionString;

        public IDataProvider Provider { get; private set; }

        #endregion

        #region "Constructor"
        
        public DataProviderFactory(DbContextOptions cfg)
        {
            _cfg = cfg;
            BuildProvider();
        }
        
        void BuildProvider()
        {

            if (Provider != null)
            {
                return;
            }

            switch (_cfg.DatabaseProvider?.ToLower())
            {
                case SqlClient:
                    {
                        Provider = new SqlProvider(_cfg.ConnectionString);
                        break;
                    }
            }
            
        }
        
        #endregion

    }

}
