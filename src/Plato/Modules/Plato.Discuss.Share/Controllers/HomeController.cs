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
        private readonly IEntityReplyStore<Reply> _replyStore;

        public HomeController(
            IEntityStore<Topic> topicStore,
            IEntityReplyStore<Reply> replyStore,
            IContextFacade contextFacade)
        {
            _topicStore = topicStore;
            _replyStore = replyStore;
            _contextFacade = contextFacade;
        }

        public async Task<IActionResult> Index(
            int entityId,
            int entityReplyId = 0)
        {

            // We always need an entity Id
            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var entity = await _topicStore.GetByIdAsync(entityId);
            if (entity == null)
            {
                return NotFound();
            }

            var baseUrl = await _contextFacade.GetBaseUrlAsync();
            var viewModel = new ShareViewModel()
            {
                EntityUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["Area"] = "Plato.Discuss",
                    ["Controller"] = "Home",
                    ["Action"] = "Topic",
                    ["Id"] = entity.Id,
                    ["Alias"] = entity.Alias
                })
            };
        
            return View(viewModel);

        }


    }
}
