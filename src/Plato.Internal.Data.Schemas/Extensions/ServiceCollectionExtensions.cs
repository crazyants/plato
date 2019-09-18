using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Internal.Data.Schemas.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataSchemas(
            this IServiceCollection services)
        {
                        
            services.AddTransient<ISchemaManager, SchemaManager>();
            services.AddTransient<ISchemaBuilder, SchemaBuilder>();

            return services;
        }


    }
}
