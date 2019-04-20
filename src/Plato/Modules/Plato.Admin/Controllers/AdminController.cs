using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Plato.Admin.Models;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Admin.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IViewProviderManager<AdminIndex> _viewProvider;

        public IStringLocalizer S { get; }
        
        public AdminController(
            IStringLocalizer stringLocalizer,
            IBreadCrumbManager breadCrumbManager,
            IViewProviderManager<AdminIndex> viewProvider)
        {

            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;

            S = stringLocalizer;

        }

        public async Task<IActionResult> Index()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"]);
            });

            return View((LayoutViewModel)await _viewProvider.ProvideIndexAsync(new AdminIndex(), this));

        }

    }

}
