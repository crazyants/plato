using Microsoft.AspNetCore.Hosting;

namespace Plato.Internal.Hosting
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
