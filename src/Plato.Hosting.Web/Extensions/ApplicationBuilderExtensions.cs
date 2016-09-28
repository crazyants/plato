using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Plato.Hosting.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {

        public static IApplicationBuilder ConfigureWebHost(
           this IApplicationBuilder app,
           IHostingEnvironment env,
           ILoggerFactory loggerFactory)
        {

      

            // ordhard code

            //loggerFactory.AddOrchardLogging(builder.ApplicationServices);

            //var extensionManager = builder.ApplicationServices.GetRequiredService<IExtensionManager>();

            //// Add diagnostices pages
            //// TODO: make this modules from configurations
            //// builder.UseRuntimeInfoPage(); // removed!
            //builder.UseDeveloperExceptionPage();

            //// Add static files to the request pipeline.

            //builder.UseStaticFiles();

            //// TODO: configure the location and parameters (max-age) per module.
            //foreach (var extension in extensionManager.AvailableExtensions())
            //{
            //    var contentPath = Path.Combine(hostingEnvironment.ContentRootPath, extension.Location, extension.Id, "Content");
            //    if (Directory.Exists(contentPath))
            //    {
            //        builder.UseStaticFiles(new StaticFileOptions()
            //        {
            //            RequestPath = "/" + extension.Id,
            //            FileProvider = new PhysicalFileProvider(contentPath)
            //        });
            //    }
            //}

            //// Ensure the shell tenants are loaded when a request comes in
            //// and replaces the current service provider for the tenant's one.
            //builder.UseMiddleware<OrchardContainerMiddleware>();

            //// Route the request to the correct tenant specific pipeline
            //builder.UseMiddleware<OrchardRouterMiddleware>();

            //// Load controllers
            //var applicationPartManager = builder.ApplicationServices.GetRequiredService<ApplicationPartManager>();

            //var sw = Stopwatch.StartNew();

            //Parallel.ForEach(extensionManager.AvailableFeatures(), feature =>
            //{
            //    try
            //    {
            //        var extensionEntry = extensionManager.LoadExtension(feature.Extension);
            //        applicationPartManager.ApplicationParts.Add(new AssemblyPart(extensionEntry.Assembly));
            //    }
            //    catch
            //    {
            //        // TODO: An extension couldn't be loaded, log
            //    }
            //});

            //var message = $"Overall time to dynamically compile and load extensions: {sw.Elapsed}";

            //if (Debugger.IsAttached)
            //{
            //    Debug.WriteLine(message);
            //}
            //else
            //{
            //    Reporter.Output.WriteLine(message);
            //    Reporter.Output.WriteLine();
            //}

            return app;
        }
    }
}
