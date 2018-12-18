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

            // We want a new timer to be created every time it's requested
            services.TryAddScoped<ISafeTimerFactory, SafeTimerFactory>();
            services.TryAddTransient<ISafeTimer, SafeTimer>();
      
            return services;

        }


    }
}
