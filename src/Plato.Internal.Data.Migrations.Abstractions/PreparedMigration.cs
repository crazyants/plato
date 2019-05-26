using System;
using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations.Abstractions
{
    public class PreparedMigration
    {
        public string Version { get; set; }

        public Version TypedVersion { get; set; }
        
        public ICollection<string> Statements { get; set; }
        
    }
}
