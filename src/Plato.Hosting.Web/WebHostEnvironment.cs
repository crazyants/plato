using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;

namespace Plato.Hosting.Web
{
    public class WebHostEnvironment : HostEnvironment
    {
        public WebHostEnvironment(
        IHostingEnvironment hostingEnvironment) : 
            base(hostingEnvironment)
        {
            //T = null; // NullLocalizer.Instance;
        }

       // public Localizer T { get; set; }

    }
}
