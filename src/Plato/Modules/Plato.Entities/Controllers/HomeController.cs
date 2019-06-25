using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout.ModelBinding;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Entities.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IEntityStore<Entity> _entityStore;
        private readonly IEntityService<Entity> _entityService;
        private readonly IContextFacade _contextFacade;
 

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer localizer,
            IEntityStore<Entity> entityStore,
            IEntityService<Entity> entityService,
            IContextFacade contextFacade)
        {

            _entityStore = entityStore;
            _entityService = entityService;
            _contextFacade = contextFacade;

            T = localizer;
            S = stringLocalizer;

        }

        // -----------------
        // Get User
        // -----------------

        public async Task<IActionResult> GetEntity(EntityOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityOptions();
            }

            if (opts.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(opts.Id));
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            
            // Get entity checking to ensure the entity is visible
            var entities = await _entityStore.QueryAsync()
                .Select<EntityQueryParams>(q =>
                {
                    q.UserId.Equals(user?.Id ?? 0);
                    q.Id.Equals(opts.Id);
                    q.HideSpam.True();
                    q.HideHidden.True();
                    q.HideDeleted.True();
                    q.HidePrivate.True();
                })
                .ToList();

            // Ensure entity exists
            if (entities?.Data == null)
            {
                return View();
            }

            // Return view
            return View(entities.Data[0]);

        }


    }

}
