using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using Plato.Modules;
using Plato.Hosting.Web.Expanders;
using System.IO;
using System.Text;
using System;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Hosting;
using Plato.Shell.Extensions;
using Plato.Data.Extensions;
using Plato.Repositories.Extensions;
using Plato.Layout;
using Plato.Modules.Extensions;
using Plato.Hosting.Extensions;
using Plato.Modules.Abstractions;
using Plato.Hosting.Web.Middleware;
using Plato.FileSystem;
using Microsoft.AspNetCore.Http;
using Plato.Hosting.Web.Routing;
using Plato.Services.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Plato.Hosting.Web.Extensions

{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlato(
            this IServiceCollection services)
        {

            // file system

            services.AddFileSystem();

            // configure tenants

            services.ConfigureShell("sites");
               
                   
            // standard .NET core extensions

            services.AddLogging();
            services.AddOptions();
            services.AddLocalization();
            
            // database

            services.AddPlatoDbContext();

            services.AddRepositories();

            services.AddServices();


            // add mvc core
            var mvcBuilder = services.AddMvcCore();
            mvcBuilder.AddViews()
                .AddViewLocalization()
                .AddRazorViewEngine()
                .AddJsonFormatters();

            services.AddLogging()
                .AddHostCore()
                .AddWebHost()
                .AddMemoryCache();

            // theme

            services.Configure<RazorViewEngineOptions>(configureOptions: options =>
            {
                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander("classic"));                
            });

            // layout

            services.AddSingleton<ILayoutManager, LayoutManager>();

            // add external modules

            services.AddModules(mvcBuilder);

            return services;

        }
        

        public static IServiceCollection AddWebHost(this IServiceCollection services)
        {
            services.AddSingleton<IHostEnvironment, WebHostEnvironment>();
            services.AddSingleton<IPlatoFileSystem, HostedFileSystem>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
           
            // serve static files within module folders

            var moduleManager = app.ApplicationServices.GetRequiredService<IModuleManager>();
            foreach (ModuleEntry moduleEntry in moduleManager.AvailableModules)
            {
                var contentPath = Path.Combine(env.ContentRootPath, moduleEntry.Descriptor.Location, moduleEntry.Descriptor.ID, "Content");
                if (Directory.Exists(contentPath))
                {
                    app.UseStaticFiles(new StaticFileOptions()
                    {
                        RequestPath = "/" + moduleEntry.Descriptor.ID,
                        FileProvider = new PhysicalFileProvider(contentPath)
                    });
                }
            }


            app.UseMiddleware<PlatoContainerMiddleware>();

            // Route the request to the correct tenant specific pipeline
            //app.UseMiddleware<PlatoRouterMiddleware>();

            var applicationPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            
            foreach (ModuleEntry moduleEntry in moduleManager.AvailableModules)
            {
                foreach (var assembly in moduleEntry.Assmeblies)
                    applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }


            // configure routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            return app;
        }

    }
    
    

}
