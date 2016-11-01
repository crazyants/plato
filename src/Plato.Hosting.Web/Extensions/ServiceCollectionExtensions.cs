using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
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

namespace Plato.Hosting.Web.Extensions

{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHost(
            this IServiceCollection services,
            Action<IServiceCollection> additionalDependencies)
        {
            services.AddFileSystem();
            additionalDependencies(services);

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
                
                internalServices.AddSingleton<IHostEnvironment, WebHostEnvironment>();
                internalServices.AddSingleton<IPlatoFileSystem, HostedFileSystem>();
                internalServices.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                internalServices.AddPlatoDbContext()
                    .AddRepositories()
                    .AddStores();
            });
        }


        public static IServiceCollection AddPlatoMvc(
            this IServiceCollection services)
        {
            // add mvc core
            services.AddMvcCore()
                .AddViews()
                .AddViewLocalization()
                .AddRazorViewEngine()
                .AddJsonFormatters();

            var moduleManager = services.BuildServiceProvider().GetService<IModuleManager>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                // view location expanders for modules
                foreach (var moduleEntry in moduleManager.AvailableModules)
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander(moduleEntry.Descriptor.ID));

                // theme
                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander("classic"));

                // ensure loaded modules are aware of current context
                var moduleAssemblies = moduleManager.AllAvailableAssemblies.Select(x => MetadataReference.CreateFromFile(x.Location)).ToList();
                var previous = options.CompilationCallback;
                options.CompilationCallback = context =>
                {
                    previous?.Invoke(context);
                    context.Compilation = context.Compilation.AddReferences(moduleAssemblies);
                };

            });
            
            return services;
        }

        public static IServiceCollection AddPlato(
            this IServiceCollection services)
        {
            services.AddWebHost();

            // configure tenants

            services.ConfigureShell("sites");

            services.AddSingleton<ILayoutManager, LayoutManager>();

            services.AddPlatoMvc();

            // Save the list of service definitions
            services.AddSingleton(_ => services);

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

            app.UseStaticFiles();
            
            app.UseMiddleware<PlatoContainerMiddleware>();

           
            // configure modules

            var applicationPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            var moduleManager = app.ApplicationServices.GetRequiredService<IModuleManager>();
            foreach (var moduleEntry in moduleManager.AvailableModules)
            {

                // serve static files within module folders
                var contentPath = Path.Combine(env.ContentRootPath, moduleEntry.Descriptor.Location,
                    moduleEntry.Descriptor.ID, "Content");
                if (Directory.Exists(contentPath))
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        RequestPath = "/" + moduleEntry.Descriptor.ID,
                        FileProvider = new PhysicalFileProvider(contentPath)
                    });
                // add modules as application parts
                foreach (var assembly in moduleEntry.Assmeblies)
                    applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }

            // Route the request to the correct tenant specific pipeline
            // app.UseMiddleware<PlatoRouterMiddleware>();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{site=Default}/{controller=Home}/{action=Index}/{id?}");
            });
            
            // configure routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });


            return app;
        }
    }
}