using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Answers.Navigation;

namespace Plato.Questions.Answers
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
          
            // Register navigation providers
            services.AddScoped<INavigationProvider, QuestionAnswerMenu>();
            services.AddScoped<INavigationProvider, QuestionAnswerDetailsMenu>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "QuestionsAddAnswer",
                areaName: "Plato.Questions.Answers",
                template: "questions/answer/add/{id}",
                defaults: new { controller = "Home", action = "ToAnswer" }
            );

            routes.MapAreaRoute(
                name: "QuestionsDeleteAnswer",
                areaName: "Plato.Questions.Answers",
                template: "questions/answer/delete/{id}",
                defaults: new { controller = "Home", action = "FromAnswer" }
            );

        }
    }
}