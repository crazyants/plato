using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;

namespace Plato.Internal.Hosting.Web.Configuration
{
    
    public class AntiForgeryOptionsConfiguration : IConfigureOptions<AntiforgeryOptions>
    {
 
        public void Configure(AntiforgeryOptions options)
        {
            options.Cookie.Name = "plato_csrf";
            options.FormFieldName = "plato-csrf";
            options.HeaderName = "X-Csrf-Token";
        }
        
    }
}
