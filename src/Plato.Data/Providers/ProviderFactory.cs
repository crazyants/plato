using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data
{
    public class ProviderFactory
    {

        protected ProviderFactory()
        {
        }

        public static IDataProvider GetProvider(string connectionString)
        {
            return new SqlProvider(connectionString);
        }

    }
}
