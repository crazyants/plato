using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Plato.Internal.Navigation.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoNavigation(
            this IServiceCollection services)
        {

            services.TryAddScoped<INavigationManager, NavigationManager>();
            
            return services;

        }


    }
}
