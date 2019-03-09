using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Migrations.Extensions;
using Plato.Internal.Data.Schemas.Extensions;

namespace Plato.Internal.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        
        public static IServiceCollection AddPlatoDbContext(
            this IServiceCollection services)
        {

            // Add default data options and data context
            // DbContextOptions is overriden for each tennet within ShellContainerFactory
            services.AddScoped<IDbContextOptions, DbContextOptions>();
            services.AddScoped<IDbContext, DbContext>();

            services.AddSingleton<IConfigureOptions<DbContextOptions>, DbContextOptionsConfigure>();
            services.AddTransient<IDbHelper, DbHelper>();

            // Parsers
            services.AddSingleton<IFullTextQueryParser, FullTextQueryParser>();

            // Add schemas
            services.AddDataSchemas();

            // Add migrations 
            services.AddDataMigrations();

            return services;

        }


    }
}
