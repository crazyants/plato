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
            //services.AddPlatoDeferredTasks();

            return services;

        }

        public static IServiceCollection AddPlatoDeferredTasks(
            this IServiceCollection services)
        {
            services.TryAddScoped<IDeferredTaskState, HttpContextTaskState>();
            services.TryAddScoped<IDeferredTaskManager, DeferredTaskManager>();
            return services;
        }
        
    }

}
