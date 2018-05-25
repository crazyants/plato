﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Hosting;
using Plato.Hosting.Extensions;
using Plato.Shell.Models;

namespace Plato.WebApi
{
    public class Startup : StartupBase
    {

        private readonly ShellSettings _shellSettings;

        public Startup(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        
            routes.MapAreaRoute(
                name: "WebAPI",
                area: "Plato.WebApi",
                template: "api/{controller}/{action}/{id?}",
                controller: "Users",
                action: "Get"
            );

        }

    }
}