using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data
{
    public class ProviderFactory
    {

        #region "Consts"

        // System.Data.SqlClient
        private const string Sql = "sql";

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
        
        public ProviderFactory(
            string connectionString,
            string providerNAme)
        {
            if (string.IsNullOrEmpty(providerNAme))
                throw new ArgumentNullException(nameof(providerNAme));

            _connectionString = ConnectionString;
            
            
            // System.Data.EntityClient            
            // System.Data.OleDb
            
            switch (providerNAme.ToLower())
            {
                case Sql:
                    {
                        _provider = new SqlProvider(connectionString);
                        break;
                    }

            }


        }

        #endregion


    }
}
