using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Razor;

namespace Plato.Core.ViewFeatures
{
    public class ModularThemeViewStart : RazorPage<dynamic>
    {

        public override async Task ExecuteAsync()
        {
            
            // Required services
            var contextFacade = this.Context.RequestServices.GetService<IContextFacade>();

            var controllerName = this.ViewContext.RouteData.Values["controller"].ToString();
            switch (controllerName)
            {
                case "Admin":
                    Layout = $"~/{await contextFacade.GetCurrentThemeAsync()}/Shared/_AdminLayout.cshtml";
                    break;
                default:
                    Layout = $"~/{await contextFacade.GetCurrentThemeAsync()}/Shared/_Layout.cshtml";
                    break;
            }
            
        }

    }

}
