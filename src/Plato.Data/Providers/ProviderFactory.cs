using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data
{
    public class ProviderFactory
    {

        private const string Sql = "sql";
        private const string SqlLite = "sqllite";
        private const string Odbc = "odbc";        
        private const string MySql = "mysql";
        private const string Oracle = "oracle";

        private string _connectionString;
        private IDataProvider _provider;
        private System.Data.IDataReader _reader;
        
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

        public ProviderFactory(
            string connectionString,
            string providerNAme)
        {
            if (string.IsNullOrEmpty(providerNAme))
                throw new ArgumentNullException(nameof(providerNAme));

            _connectionString = ConnectionString;
                     
            switch (providerNAme.ToLower())
            {
                case Sql:
                    {
                        _provider = new SqlProvider(connectionString);
                        break;
                    }

            }


        }



    }
}
