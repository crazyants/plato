using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions
{
    public abstract class StartupBase : IStartup
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {

        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider)
        {

        }
    }
}
