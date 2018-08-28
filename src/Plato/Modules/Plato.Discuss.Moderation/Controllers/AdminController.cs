using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Moderation.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Moderation.Models;

namespace Plato.Discuss.Moderation.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        private readonly IContextFacade _contextFacade;
        private readonly IViewProviderManager<Moderator> _viewProvider;
        private readonly IAlerter _alerter;


        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }


        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IContextFacade contextFacade,
            IViewProviderManager<Moderator> viewProvider,
            IAlerter alerter)
        {
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index()
        {
            
            var model = await _viewProvider.ProvideIndexAsync(new Moderator(), this);
            return View(model);

        }
        
        public async Task<IActionResult> Create()
        {
         
            var model = await _viewProvider.ProvideEditAsync(new Moderator(), this);
            return View(model);

        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost()
        {

            var result = await _viewProvider.ProvideUpdateAsync(new Moderator(), this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Moderator Created Successfully!"]);

            return RedirectToAction(nameof(Index));


        }

    }

}
