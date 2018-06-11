using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Plato.Internal.FileSystem
{
    public class HostedFileSystem : PlatoFileSystem
    {
        public HostedFileSystem(
            IHostingEnvironment hostingEnvironment,
            ILogger<HostedFileSystem> logger) :
            base(
                hostingEnvironment.ContentRootPath,
                hostingEnvironment.ContentRootFileProvider,
                logger)
        { }
    }
}
