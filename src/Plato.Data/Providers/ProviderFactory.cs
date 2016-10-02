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

        public static IDataProvider GetProvider(
            string connectionString,
            string providerNAme)
        {

            if (string.IsNullOrEmpty(providerNAme))
                throw new ArgumentNullException(nameof(providerNAme));

            // encapculate the logic for each database within it's
            // own implemnetation deriving from IDataProvider

            switch (providerNAme.ToLower())
            {
                case Sql:
                    return new SqlProvider(connectionString);
            }

            throw new Exception("No data provider has been configured");

        }

    }
}
