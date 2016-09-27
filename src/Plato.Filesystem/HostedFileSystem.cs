using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Plato.FileSystem
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
