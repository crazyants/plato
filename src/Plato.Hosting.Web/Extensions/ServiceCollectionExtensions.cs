using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Plato.Data.Extensions;
using Plato.FileSystem;
using Plato.Hosting.Extensions;
using Plato.Hosting.Web.Expanders;
using Plato.Hosting.Web.Middleware;
using Plato.Layout;
using Plato.Modules;
using Plato.Modules.Abstractions;
using Plato.Modules.Extensions;
using Plato.Repositories.Extensions;
using Plato.Shell.Extensions;
using Plato.Stores.Extensions;
using Plato.Cache.Extensions;
using Plato.Hosting.Web.Routing;
using Plato.Models.Roles;
using Plato.Models.Users;
using Plato.Modules.Expanders;

namespace Plato.Hosting.Web.Extensions

{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHost(
            this IServiceCollection services,
            Action<IServiceCollection> configure)
        {
            services.AddFileSystem();

            configure(services);

            // Let the app change the default tenant behavior and set of features
            configure?.Invoke(services);

            // Register the list of services to be resolved later on
            services.AddSingleton(_ => services);

            return services;

        }

        public static IServiceCollection AddWebHost(this IServiceCollection services)
        {
            return services.AddHost(internalServices =>
            {
                internalServices.AddLogging();
                internalServices.AddOptions();
                internalServices.AddLocalization();
                internalServices.AddHostCore();
                internalServices.AddModules();
                internalServices.AddCaching();
                
                internalServices.AddSingleton<IHostEnvironment, WebHostEnvironment>();
                internalServices.AddSingleton<IPlatoFileSystem, HostedFileSystem>();
                internalServices.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                
                internalServices.AddPlatoDbContext();
                internalServices.AddRepositories();
                internalServices.AddStores();
                
            });

         


        }

        public static IServiceCollection AddPlato(
            this IServiceCollection services)
        {

            // add shell services

            services.AddWebHost();
            
            // configure shell

            services.ConfigureShell("sites");
            
            // add auth
            
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async context =>
                        {
                            await SecurityStampValidator.ValidatePrincipalAsync(context);
                        }
                    };
                })
                .AddCookie(IdentityConstants.ExternalScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.ExternalScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                })
                .AddCookie(IdentityConstants.TwoFactorUserIdScheme, IdentityConstants.TwoFactorUserIdScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                });
            
            // add mvc

            services.AddPlatoMvc();
            
            return services;
        }
        
        public static IServiceCollection AddPlatoMvc(
            this IServiceCollection services)
        {

            var moduleManager = services.BuildServiceProvider().GetService<IModuleManager>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                // view location expanders for modules

                foreach (var moduleEntry in moduleManager.AvailableModules)
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander(moduleEntry.Descriptor.ID));

                // theme

                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander("classic"));
                
                // ensure loaded modules are aware of current context

                var moduleAssemblies = moduleManager.AllAvailableAssemblies
                    .Where(x => !x.IsDynamic)
                    .Select(x => MetadataReference.CreateFromFile(x.Location)).ToList();
                var previous = options.CompilationCallback;
                options.CompilationCallback = context =>
                {
                    previous?.Invoke(context);
                    context.Compilation = context.Compilation.AddReferences(moduleAssemblies);
                };
                
            });

       
            // add mvc core services
            services.AddMvcCore()
                .AddViews()
                .AddViewLocalization()
                .AddRazorViewEngine()
                .AddJsonFormatters();
            
            return services;

        }
        
        public static IApplicationBuilder UsePlato(
            this IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(IConfiguration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // add authentication middleware
            app.UseAuthentication();

            // load static file middleware
            app.UseStaticFiles();

            // load module controllers
            var applicationPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            var moduleManager = app.ApplicationServices.GetRequiredService<IModuleManager>();
            foreach (var moduleEntry in moduleManager.AvailableModules)
            {

                // serve static files within module folders
                var contentPath = Path.Combine(env.ContentRootPath, 
                    moduleEntry.Descriptor.Location,
                    moduleEntry.Descriptor.ID, "Content");
                if (Directory.Exists(contentPath))
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        RequestPath = "/" + moduleEntry.Descriptor.ID.ToLower() + "/content",
                        FileProvider = new PhysicalFileProvider(contentPath)
                    });
                }

                // add modules as application parts
                foreach (var assembly in moduleEntry.Assmeblies)
                {
                    applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));
                }
                    
           
            }
            
            // create services container for each shell
            app.UseMiddleware<PlatoContainerMiddleware>();

            // create uniuqe pipeline for each shell
            app.UseMiddleware<PlatoRouterMiddleware>();
        
            return app;
        }
    }
}