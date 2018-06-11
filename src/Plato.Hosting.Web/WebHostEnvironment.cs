using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting;

namespace Plato.Hosting.Web
{
    public class WebHostEnvironment : HostEnvironment
    {
        public WebHostEnvironment(
        IHostingEnvironment hostingEnvironment) : 
            base(hostingEnvironment)
        {
            T = null; 
        }

       public IStringLocalizer T { get; set; }

    }
}
