using Plato.Data.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Plato.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoDbContext(
            this IServiceCollection services)
        {

            //services.AddSingleton<IDbContextOptions, DbContextOptions>();

            services.AddSingleton<IConfigureOptions<DbContextOptions>, DbContextOptionsConfigure>();         
            services.AddTransient<IDbContextt, DbContext>();

            services.AddTransient<IDataMigrationManager, DataMigrationManager>();
            services.AddTransient<AutomaticDataMigrations>();

            return services;

        }


    }
}
