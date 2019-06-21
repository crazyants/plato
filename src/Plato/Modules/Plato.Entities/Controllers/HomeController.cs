using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Titles;

namespace Plato.Entities.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IEntityStore<Entity> _entityStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer localizer,
            IEntityStore<Entity> entityStore)
        {

            _entityStore = entityStore;

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

            // Get entity
            var entity = await _entityStore.GetByIdAsync(opts.Id);

            // Ensure entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Return view
            return View(entity);

        }


    }

}
