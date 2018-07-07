using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoTasks(
            this IServiceCollection services)
        {

            services.TryAddScoped<ISafeTimer, SafeTimer>();

            services.TryAddScoped<IBackgroundTaskManager, BackgroundTaskManager>();
            
            return services;

        }


    }
}
