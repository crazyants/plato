namespace Plato.Data
{

    public class DbContextOptions 
    {

        public string DatabaseProvider { get; set; }

        public string ConnectionString { get; set; }

        public string TablePrefix { get; set; }

        public DbContextOptions()
        {

        }

    }
}
