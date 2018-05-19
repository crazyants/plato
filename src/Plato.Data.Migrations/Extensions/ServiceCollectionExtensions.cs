using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Plato.Data.Migrations.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataMigrations(
            this IServiceCollection services)
        {

            //services.AddSingleton<IConfigureOptions<DbContextOptions>, DbContextOptionsConfigure>();         
            //services.AddTransient<IDbContext, DbContext>();

            services.AddTransient<IDataMigrationManager, DataMigrationManager>();
            services.AddTransient<AutomaticDataMigrations>();

            return services;
        }


    }
}
