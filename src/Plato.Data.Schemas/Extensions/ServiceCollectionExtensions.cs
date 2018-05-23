using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Schemas.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataSchemas(
            this IServiceCollection services)
        {

            services.AddTransient<ISchemaLoader, SchemaLoader>();
            services.AddTransient<ISchemaProvider, SchemaProvider>();
            services.AddTransient<ISchemaBuilder, SchemaBuilder>();

            return services;
        }


    }
}
