using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Migrations
{
    public class DataMigration
    {

        public string InstallSql { get; set; }

        public string UpgradeSql { get; set; }

        public string RollBackSql { get; set; }

        public string Version { get; set; }

    }
}
