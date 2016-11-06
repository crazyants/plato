using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Plato.Hosting;
using Plato.Hosting.Extensions;
using Plato.Models.Roles;
using Plato.Models.Users;
using Plato.Shell.Models;
using Plato.Stores.Roles;
using Plato.Stores.Users;

namespace Plato.WebApi
{
    public class Startup : StartupBase
    {
        private readonly IdentityOptions _options;

        private readonly string _tenantName;
        private readonly string _tenantPrefix;


        public Startup(
            ShellSettings shellSettings,
            IOptions<IdentityOptions> options)
        {
            _options = options.Value;
            _tenantName = shellSettings.Name;
            _tenantPrefix = shellSettings.RequestedUrlPrefix;
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
                template: "api/{controller}/{action}",
                controller: "Users",
                action: "Get"
            );


        }
    }
}