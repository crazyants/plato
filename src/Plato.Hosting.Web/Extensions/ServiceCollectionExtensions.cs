using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using Plato.Environment.Modules;
using Plato.Hosting.Web.Expanders;
using Plato.Environment.Shell;
using System.IO;
using System.Text;
using System;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Plato.Environment.Shell.Extensions;
using Plato.Data.Extensions;
using Plato.Repositories.Extensions;
using Plato.Data;
using Plato.Layout;

namespace Plato.Hosting.Web.Extensions

{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlato(
            this IServiceCollection services)
        {
                    
            services.AddShell();
        
            // database

            services.AddPlatoDbContext();
            services.AddRepositories();
                                   
            //services.AddSingleton<ILogger, Logger>();
                        
            services.AddSingleton<IHostEnvironment, WebHostEnvironment>();

            // shell context (settings, features etc)

            services.AddSingleton<IShellContextFactory, ShellContextFactory>();

            // implement our own IHostEnvironment 

            //services.AddSingleton<IHostEnvironment, WebHostEnvironment>();


            // mvc
            var mvcBuilder = services.AddMvc();

            // theme

            //var moduleManager = services.BuildServiceProvider().GetService<IModuleManager>();
            services.Configure<RazorViewEngineOptions>(configureOptions: options =>
            {

                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander("classic"));
                
            });

            // layout

            services.AddSingleton<ILayoutManager, LayoutManager>();

            services.AddModules(mvcBuilder);

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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

     
            return app;
        }

    }
    

    public class ContentFileInfo : IFileInfo
    {
        private readonly byte[] _content;
        private readonly string _name;

        public ContentFileInfo(string name, string content)
        {
            _name = name;
            _content = Encoding.UTF8.GetBytes(content);
        }

        public bool Exists
        {
            get
            {
                return true;
            }
        }

        public bool IsDirectory
        {
            get
            {
                return false;
            }
        }

        public DateTimeOffset LastModified
        {
            get
            {
                return DateTimeOffset.MinValue;
            }
        }

        public long Length
        {
            get
            {
                return _content.Length;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string PhysicalPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(_content);
        }
    }

    public class ThemingFileProvider : IFileProvider
    {
        private readonly ContentFileInfo _viewStartFileInfo;
        private readonly ContentFileInfo _viewImportsFileInfo;
        private readonly ContentFileInfo _layoutFileInfo;

        public ThemingFileProvider()
        {
            _viewStartFileInfo = new ContentFileInfo("_ViewStart.cshtml", "@{ Layout = \"~/Views/Shared/_Layout.cshtml\"; }");
            _viewImportsFileInfo = new ContentFileInfo("_ViewImports.cshtml", "@inherits Orchard.DisplayManagement.Razor.RazorPage<TModel>");
            _layoutFileInfo = new ContentFileInfo("_Layout.cshtml", @"
                  Hello
                ");
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == "/_ViewStart.cshtml")
            {
                return _viewStartFileInfo;
            }
            else if (subpath == "/Views/_ViewImports.cshtml")
            {
                return _viewImportsFileInfo;
            }
            else if (subpath == "/Views/Shared/_Layout.cshtml")
            {
                return _layoutFileInfo;
            }

            return null;
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }




}
