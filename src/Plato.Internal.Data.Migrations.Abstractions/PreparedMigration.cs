using System;
using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations.Abstractions
{
    public class PreparedMigration
    {

        public string ModuleId { get; set; }

        public string Version { get; set; }

        public Version TypedVersion => new Version(this.Version);

        public ICollection<string> Statements { get; set; }
        
    }
}
