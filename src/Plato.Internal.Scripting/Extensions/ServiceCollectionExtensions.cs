using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.Internal.Scripting.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoScripting(
            this IServiceCollection services)
        {

            services.TryAddScoped<IScriptManager, ScriptManager>();
            
            return services;

        }


    }
}
