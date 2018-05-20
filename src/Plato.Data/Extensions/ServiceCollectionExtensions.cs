using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Data.Abstractions.Schemas;
using Plato.Data.Migrations;
using Plato.Data.Schemas.Extensions;

namespace Plato.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoDbContext(
            this IServiceCollection services)
        {

            // add schemas

            services.AddSchemas();

            // add migrations 

            services.AddTransient<IDataMigrationManager, DataMigrationManager>();
            services.AddTransient<AutomaticDataMigrations>();

            return services;

        }


    }
}
