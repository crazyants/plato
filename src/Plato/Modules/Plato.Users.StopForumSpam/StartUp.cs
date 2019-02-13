using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.ViewProviders;
using Plato.Users.StopForumSpam.SpamOperations;

namespace Plato.Users.StopForumSpam
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
            
            // Register spam operation type providers
            services.AddScoped<ISpamOperationTypeProvider<SpamOperationType>, SpamOperationTypes>();
            
            // Register spam operation manager for users
            services.AddScoped<ISpamOperationManager<User>, SpamOperationManager<User>>();

            // Register spam operation providers
            services.AddScoped<ISpamOperationProvider<User>, RegistrationOperation>();
            services.AddScoped<ISpamOperationProvider<User>, LoginOperation>();

            //// Login view provider
            //services.AddScoped<IViewProviderManager<UserLogin>, ViewProviderManager<UserLogin>>();
            //services.AddScoped<IViewProvider<UserLogin>, LoginViewProvider>();

            // Register view provider
            services.AddScoped<IViewProviderManager<UserRegistration>, ViewProviderManager<UserRegistration>>();
            services.AddScoped<IViewProvider<UserRegistration>, RegisterViewProvider>();



        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}