using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Plato
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-2.2#set-up-a-host
            // We create our own minimal host builder, not calling WebHost.CreateDefaultBuilder(args)

            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())

                .UseIISIntegration()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
