
namespace Plato.Data.Abstractions.Schemas
{

    public class Schema
    {
        
        public string Version { get; set; }

        public string InstallSql { get; set; }

        public string UpgradeSql { get; set; }

        public string RollbackSql { get; set; }

    }
}
