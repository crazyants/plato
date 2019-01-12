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
            
            services.TryAddScoped<IBackgroundTaskManager, BackgroundTaskManager>();
            services.TryAddScoped<ISafeTimerFactory, SafeTimerFactory>();
            services.TryAddScoped<IDeferredTaskManager, DeferredTaskManager>();

            return services;

        }


    }
}
