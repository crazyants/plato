using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Data
{
    public class DbContextOptionsConfigure : IConfigureOptions<DbContextOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DbContextOptionsConfigure()
        {
        }

        public DbContextOptionsConfigure(
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(DbContextOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {

                var configuration = scope.ServiceProvider.GetRequiredService<IConfigurationRoot>();

                // default configuration
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                options.DatabaseProvider = "SqlClient";
                options.TablePrefix = "";

            }

        }
    }
}