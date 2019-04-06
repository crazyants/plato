using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;

namespace Plato.Docs.ViewComponents
{

    public class DocViewComponent : ViewComponent
    {

        private readonly IEntityStore<Doc> _entityStore;
        private readonly IEntityReplyStore<DocComment> _entityReplyStore;

        public DocViewComponent(
            IEntityReplyStore<DocComment> entityReplyStore,
            IEntityStore<Doc> entityStore)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityOptions options)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            return View(await GetViewModel(options));

        }

        async Task<EntityViewModel<Doc, DocComment>> GetViewModel(
            EntityOptions options)
        {

            if (options.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Id));
            }

            var topic = await _entityStore.GetByIdAsync(options.Id);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }

            // Return view model
            return new EntityViewModel<Doc, DocComment>
            {
                Options = options,
                Entity = topic
            };

        }

    }

}
