using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Discuss.Share.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Discuss.Share.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _topicStore;
      
        public HomeController(
            IEntityStore<Topic> topicStore,
            IContextFacade contextFacade)
        {
            _topicStore = topicStore;
            _contextFacade = contextFacade;
        }

        // Share dialog

        public async Task<IActionResult> Share(int id, int offset = 0)
        {

            // We always need an entity Id
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // We always need an entity
            var entity = await _topicStore.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
      
            // Build route values
            var routeValues = new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss",
                ["Controller"] = "Home",
                ["Action"] = "Topic",
                ["Id"] = entity.Id,
                ["Alias"] = entity.Alias
            };

            // Append offset
            if (offset > 0)
            {
                routeValues.Add("offset", offset);
            }

            // Build view model
            var baseUrl = await _contextFacade.GetBaseUrlAsync();
            var viewModel = new ShareViewModel
            {
                EntityUrl = baseUrl + _contextFacade.GetRouteUrl(routeValues)
            };

            return View(viewModel);

        }
        
    }

}
