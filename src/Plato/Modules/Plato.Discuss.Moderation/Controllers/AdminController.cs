using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Plato.Discuss.Moderation.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Moderation.Models;

namespace Plato.Discuss.Moderation.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        private readonly IContextFacade _contextFacade;
        private readonly IViewProviderManager<Moderator> _viewProvider;

        public AdminController(
            IContextFacade contextFacade,
            IViewProviderManager<Moderator> viewProvider)
        {
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
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
        
    }

}
