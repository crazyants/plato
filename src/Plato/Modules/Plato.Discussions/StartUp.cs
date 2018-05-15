//using System;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.DependencyInjection;
//using Plato.Hosting;
//using Plato.Hosting.Extensions;
//using Plato.Shell.Models;

//namespace Plato.Discussions
//{
//    public class Startup : StartupBase
//    {
//        private readonly ShellSettings _shellSettings;

//        public Startup(ShellSettings shellSettings)
//        {
//            _shellSettings = shellSettings;
//        }

//        public override void ConfigureServices(IServiceCollection services)
//        {

//        }

//        public override void Configure(
//            IApplicationBuilder app,
//            IRouteBuilder routes,
//            IServiceProvider serviceProvider)
//        {

//            routes.MapAreaRoute(
//                name: "Discussions",
//                area: "Plato.Discussions",
//                template: "discussions/{controller}/{action}/{id?}",
//                controller: "Discussions",
//                action: "Index"
//            );

//        }
//    }
//}