using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Abstractions.Stores;
using Plato.Layout.Razor;

namespace Plato.Layout.Theming
{
    public class ThemeViewStart : RazorPage<dynamic>
    {
 
        public override async Task ExecuteAsync()
        {

            var store = this.Context.RequestServices.GetService<ISiteSettingsStore>();
            var siteSettings = await store.GetAsync();

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
