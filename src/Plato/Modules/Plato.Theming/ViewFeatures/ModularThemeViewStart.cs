using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Razor;

namespace Plato.Theming.ViewFeatures
{
    //public class ModularThemeViewStart : RazorPage<dynamic>
    //{

    //    public override async Task ExecuteAsync()
    //    {

    //        // Compute layout based on controller type

    //        // Required services
    //        var contextFacade = this.Context.RequestServices.GetService<IContextFacade>();

    //        var controllerName = this.ViewContext.RouteData.Values["controller"].ToString();
    //        switch (controllerName)
    //        {
    //            case "Admin":
    //                Layout = $"~/{await contextFacade.GetCurrentThemeAsync()}/Shared/_AdminLayout.cshtml";
    //                break;
    //            default:
    //                Layout = $"~/{await contextFacade.GetCurrentThemeAsync()}/Shared/_Layout.cshtml";
    //                break;
    //        }


    //    }


    //}

}
