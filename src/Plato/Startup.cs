using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Hosting.Web.Extensions;
using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace Plato
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            
            Configuration = builder.Build();

        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("F:\\Files\\wwwroot_test\\Plato2\\secrets\\"));

            services.AddPlato();
            

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory)
        {
           
            app.UsePlato(env, loggerFactory);
        }
        
    }

}
