using System;
using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations
{
    public class PreparedMigration
    {
        public string Version { get; set; }

        public Version TypedVersion { get; set; }

        public List<string> Statements { get; set; }
        
    }
}
