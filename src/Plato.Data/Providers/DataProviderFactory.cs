using Microsoft.Extensions.Logging;
using System;

namespace Plato.Data
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

        private string _connectionString;
        private string _providerName;
        private IDataProvider _provider;

        #endregion

        #region "Public Properties"
        
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }
        
        public IDataProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        #endregion

        #region "Constructor"
        
        public DataProviderFactory(            
            string connectionString,
            string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException(nameof(providerName));
                   
            _connectionString = connectionString;
            _providerName = providerName;

            BuildDataProvider();

        }


        void BuildDataProvider()
        {

            switch (_providerName.ToLower())
            {
                case SqlClient:
                    {
                        _provider = new SqlProvider(_connectionString);
                        break;
                    }
            }


        }

        #endregion


    }
}
