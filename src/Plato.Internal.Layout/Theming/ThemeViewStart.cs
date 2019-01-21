using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Layout.Razor;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Internal.Layout.Theming
{
    public class ThemeViewStart : RazorPage<dynamic>
    {
 
        public override async Task ExecuteAsync()
        {
            
            // else compute layout based on controller type
            var store = this.Context.RequestServices.GetService<ISiteSettingsStore>();
            var siteSettings = await store.GetAsync();

            // we don't have any site settings during set-p
            if (siteSettings != null)
            {
                var controllerName = this.ViewContext.RouteData.Values["controller"].ToString();
                switch (controllerName)
                {
                    case "Admin":
                        Layout = "~/Themes/" + siteSettings.ThemeName
                                             + "/Shared/_AdminLayout.cshtml";
                        break;
                    default:
                        Layout = "~/Themes/" + siteSettings.ThemeName
                                             + "/Shared/_Layout.cshtml";
                        break;
                }
            }

        }


    }

}
