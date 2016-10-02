using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data
{


    public class DbContextOptions 
    {

        public string ProviderName { get; set; }

        public string ConnectionString { get; set; }

        public DbContextOptions()
        {

        }

    }
}
