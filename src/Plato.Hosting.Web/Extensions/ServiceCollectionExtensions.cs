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
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Plato.Shell.Models;
using Plato.Data;
using Plato.Shell;

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
                    .AddServices();

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

            services.Configure<RazorViewEngineOptions>(configureOptions: options =>
            {

                // view location expanders for modules
                foreach (ModuleEntry moduleEntry in moduleManager.AvailableModules)
                {
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander(moduleEntry.Descriptor.ID));
                }

                //((List<MetadataReference>)options.AdditionalCompilationReferences).AddRange(extensionLibraryService.MetadataReferences());

                // invoke context for model binding within dynamic modules                                
                var moduleAssemblies = moduleManager.AllAvailableAssemblies.Select(x => MetadataReference.CreateFromFile(x.Location)).ToList();
                var previous = options.CompilationCallback;
                options.CompilationCallback = (context) =>
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

            services.ConfigureShell("sites")
                .AddModules()          
                .AddSingleton<ILayoutManager, LayoutManager>();
            
            services.AddPlatoMvc();

            // Save the list of service definitions
            services.AddSingleton(_ => services);

            return services;

        }
        

        //public static IServiceCollection AddWebHost(this IServiceCollection services)
        //{

        //    services.AddSingleton<IHostEnvironment, WebHostEnvironment>();
        //    services.AddSingleton<IPlatoFileSystem, HostedFileSystem>();
        //    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
        //    services.Configure<RazorViewEngineOptions>(configureOptions: options =>
        //    {
        //        options.ViewLocationExpanders.Add(new ThemeViewLocationExpander("classic"));
        //    });


        //    return services;
        //}
        

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
           // app.UseMiddleware<PlatoRouterMiddleware>();

            var applicationPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();            
            foreach (ModuleEntry moduleEntry in moduleManager.AvailableModules)
            {
                foreach (var assembly in moduleEntry.Assmeblies)
                    applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }
                               

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{site=Default}/{controller=Home}/{action=Index}/{id?}");
            });


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
