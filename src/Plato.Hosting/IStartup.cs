using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Plato.Hosting
{
    public interface IStartup
    {
   
        /// <param name="services">The collection of service descriptors.</param>
        void ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider);
    }
}
