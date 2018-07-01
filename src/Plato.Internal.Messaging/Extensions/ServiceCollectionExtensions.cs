using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Messaging.Abstractions;


namespace Plato.Internal.Messaging.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoMessaging(
            this IServiceCollection services)
        {

            services.AddSingleton<IBroker, Broker>();

            return services;

        }


    }

}
