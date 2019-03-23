using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Votes.Navigation;

namespace Plato.Questions.Votes
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
          
            // Register navigation provider
            services.AddScoped<INavigationProvider, QuestionDetailsMenu>();
            services.AddScoped<INavigationProvider, AnswerDetailsMenu>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "QuestionVotessWebApi",
                areaName: "Plato.Questions.Votes",
                template: "api/questions/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );


        }
    }
}