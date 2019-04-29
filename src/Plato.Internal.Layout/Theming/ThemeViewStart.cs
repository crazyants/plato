using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Razor;

namespace Plato.Internal.Layout.Theming
{
    public class ThemeViewStart : RazorPage<dynamic>
    {

        public override Task ExecuteAsync()
        {

            // Compute layout based on controller type

            // Required services
            var contextFacade = this.Context.RequestServices.GetService<IContextFacade>();

            var themeOptions = this.Context.RequestServices.GetService<IOptions<ThemeOptions>>();
            var path = $"{themeOptions.Value.VirtualPathToThemesFolder.ToLower()}/default";

            var controllerName = this.ViewContext.RouteData.Values["controller"].ToString();
            switch (controllerName)
            {
                case "Admin":
                    Layout = $"~/{path}/Shared/_AdminLayout.cshtml";
                    break;
                default:
                    Layout = $"~/{path}/Shared/_Layout.cshtml";
                    break;
            }

            return Task.CompletedTask;

        }


    }

}
