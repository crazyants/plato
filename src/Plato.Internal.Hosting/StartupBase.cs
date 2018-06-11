using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Hosting
{
    public abstract class StartupBase : IStartup
    {
     
        public virtual void ConfigureServices(IServiceCollection services)
        {

        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider)
        {

        }
    }
}
