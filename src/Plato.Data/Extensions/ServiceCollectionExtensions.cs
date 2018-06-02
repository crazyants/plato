using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Abstractions.Query;
using Plato.Abstractions.Shell;
using Plato.Data.Abstractions;
using Plato.Data.Migrations.Extensions;
using Plato.Data.Schemas.Extensions;

namespace Plato.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        
        public static IServiceCollection AddPlatoDbContext(
            this IServiceCollection services)
        {

            // Add default data options and data context
            // DbContextOptions is overriden for each tennet within ShellContainerFactory
            services.AddScoped<IDbContextOptions, DbContextOptions>();
            services.AddSingleton<IConfigureOptions<DbContextOptions>, DbContextOptionsConfigure>();
            services.AddScoped<IDbContext, DbContext>();
            
            // Add schemas
            services.AddDataSchemas();

            // Add migrations 
            services.AddDataMigrations();

            return services;

        }


    }
}
