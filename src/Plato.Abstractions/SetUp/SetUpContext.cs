using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Abstractions.SetUp
{
    public class SetUpContext
    {

        public string SiteName { get; set; }

        public string DatabaseProvider { get; set; }

        public string DatabaseConnectionString { get; set; }

        public string DatabaseTablePrefix { get; set; }

        public string AdminEmail { get; set; }


        public string AdminUsername { get; set; }

        public string AdminPassword { get; set; }
        
        public IDictionary<string, string> Errors { get; set; }


    }
}
