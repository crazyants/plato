
namespace Plato.Abstractions.Data
{
    public interface IDbContextOptions
    {

        string DatabaseProvider { get; set; }

        string ConnectionString { get; set; }

        string TablePrefix { get; set; }

    }

}
