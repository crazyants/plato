using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.SetUp.Services
{
    public class SetUpContext
    {

        public string SiteName { get; set; }

        public string DatabaseProvider { get; set; }

        public string DatabaseConnectionString { get; set; }

        public string DatabaseTablePrefix { get; set; }

        public List<Exception> Errors { get; set; }

    }
}
