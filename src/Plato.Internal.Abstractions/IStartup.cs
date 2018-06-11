using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Abstractions
{
    public interface IStartup
    {
   
        /// <param name="services">The collection of service descriptors.</param>
        void ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider);
    }
}
