using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
using Plato.Modules.Abstractions;
using Plato.Modules.Extensions;
using Plato.Repositories.Extensions;
using Plato.Shell.Extensions;
using Plato.Stores.Extensions;
using Plato.Cache.Extensions;
using Plato.Hosting.Web.Routing;
using Plato.Modules.Expanders;
using Plato.Security.Extensions;
using Plato.Logging.Extensions;
using Plato.Navigation.Extensions;

namespace Plato.Hosting.Web.Extensions

{
    public static class ServiceCollectionExtensions
    {

        private static IServiceCollection _services;

        public static IServiceCollection AddHost(
            this IServiceCollection services,
            Action<IServiceCollection> configure)
        {
            services.AddFileSystem();

            //configure(services);

            // Let the app change the default tenant behavior and set of features
            configure?.Invoke(services);

            // Register the list of services to be resolved later on
            services.AddSingleton(_ => services);

            return services;

        }

        public static IServiceCollection AddPlatoHost(this IServiceCollection services)
        {
            return services.AddHost(internalServices =>
            {
                internalServices.AddLogging();
                internalServices.AddOptions();
                internalServices.AddLocalization();
                internalServices.AddPlatoHostCore();
                internalServices.AddPlatoModules();
                internalServices.AddCaching();
                internalServices.AddPlatoNavigation();

                internalServices.AddSingleton<IHostEnvironment, WebHostEnvironment>();
                internalServices.AddSingleton<IPlatoFileSystem, HostedFileSystem>();
                internalServices.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                internalServices.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

                internalServices.AddPlatoLogging();

                internalServices.AddPlatoDbContext();
                internalServices.AddRepositories();
                internalServices.AddStores();
                
            });
            
        }

        public static IServiceCollection AddPlato(
            this IServiceCollection services)
        {

            // add host services

            services.AddPlatoHost();
            
            // configure host 

            services.ConfigureShell("Sites", "Schemas");
            
            // add security

            services.AddPlatoSecurity();

            // add auth

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = new PathString("/Users/Account/Login");
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
            
            // allows us to display all registered services in development mode
            _services = services;

            return services;
        }
        
        public static IServiceCollection AddPlatoMvc(
            this IServiceCollection services)
        {

            // add modules

            var moduleManager = services.BuildServiceProvider().GetService<IModuleManager>();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                // view location expanders for modules

                foreach (var moduleEntry in moduleManager.AvailableModules.Reverse())
                {
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander(moduleEntry.Descriptor.ID));
                }
                    

                // view location expanders for theme

                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander("classic"));

                // ensure loaded modules are aware of current context
                
                var moduleReferences = moduleManager.AllAvailableAssemblies
                    .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
                    .Select(x => MetadataReference.CreateFromFile(x.Location))
                    .ToList();

                // https://github.com/aspnet/Mvc/issues/4497
                // https://github.com/aspnet/Razor/issues/834
                foreach (var moduleReference in moduleReferences)
                {
                    options.AdditionalCompilationReferences.Add(moduleReference);
                }
                
            });
            
            // add mvc core services

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            var builder = services.AddMvcCore()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddViews()
                .AddRazorViewEngine();

            // add default framework parts
            var applicationPartManager = services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
            foreach (var moduleEntry in moduleManager.AvailableModules)
            {
                // add modules as application parts
                foreach (var assembly in moduleEntry.Assmeblies)
                {
                    applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));
                }
            }

            AddDefaultFrameworkParts(builder.PartManager);

            // add json formatter

            builder.AddJsonFormatters();

            return services;

        }
        
        public static IApplicationBuilder UsePlato(
            this IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory logger)
        {

            if (env.IsDevelopment())
            {
                logger.AddConsole();
                logger.AddDebug();
                
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                ListAllRegisteredServices(app);
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

            }
            
            // create services container for each shell
            app.UseMiddleware<PlatoContainerMiddleware>();

            // create uniuqe pipeline for each shell
            app.UseMiddleware<PlatoRouterMiddleware>();
        
            return app;
        }

        private static void AddDefaultFrameworkParts(ApplicationPartManager partManager)
        {
            var mvcTagHelpersAssembly = typeof(InputTagHelper).Assembly;
            if (!partManager.ApplicationParts.OfType<AssemblyPart>().Any(p => p.Assembly == mvcTagHelpersAssembly))
            {
                partManager.ApplicationParts.Add(new AssemblyPart(mvcTagHelpersAssembly));
            }

            var mvcRazorAssembly = typeof(UrlResolutionTagHelper).Assembly;
            if (!partManager.ApplicationParts.OfType<AssemblyPart>().Any(p => p.Assembly == mvcRazorAssembly))
            {
                partManager.ApplicationParts.Add(new AssemblyPart(mvcRazorAssembly));
            }
        }
        
        private static void ListAllRegisteredServices(IApplicationBuilder app)
        {
            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }

    }
}