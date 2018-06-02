
namespace Plato.Abstractions.Data
{

    public class DbContextOptions  : IDbContextOptions
    {

        public string DatabaseProvider { get; set; }

        public string ConnectionString { get; set; }

        public string TablePrefix { get; set; }

    }
}
