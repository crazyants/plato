using Microsoft.Extensions.DependencyInjection;
using Plato.Data.Migrations.Extensions;
using Plato.Data.Schemas.Extensions;

namespace Plato.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoDbContext(
            this IServiceCollection services)
        {

            // add schemas

            services.AddDataSchemas();

            // add migrations 

            services.AddDataMigrations();

            return services;

        }


    }
}
