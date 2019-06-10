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
            
            // Background tasks
            services.TryAddScoped<IBackgroundTaskManager, BackgroundTaskManager>();
            services.TryAddSingleton<ISafeTimerFactory, SafeTimerFactory>();

            // Deferred tasks
            services.TryAddSingleton<IDeferredTaskState, SingletonTaskState>();
            services.TryAddScoped<IDeferredTaskManager, DeferredTaskManager>();
            //services.TryAddScoped<IDeferredTaskState, HttpContextTaskState>();

            return services;

        }

    }

}
