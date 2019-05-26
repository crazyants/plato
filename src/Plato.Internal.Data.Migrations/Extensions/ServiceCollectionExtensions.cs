using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Data.Migrations.Abstractions;

namespace Plato.Internal.Data.Migrations.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataMigrations(
            this IServiceCollection services)
        {

            services.AddTransient<IDataMigrationManager, DataMigrationManager>();
            services.AddTransient<IDataMigrationBuilder, DataMigrationBuilder>();
            services.AddTransient<AutomaticDataMigrations>();

            return services;
        }


    }
}
