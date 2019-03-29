using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;

namespace Plato.Ideas.ViewComponents
{

    public class IdeaViewComponent : ViewComponent
    {

        private readonly IEntityStore<Idea> _entityStore;
        private readonly IEntityReplyStore<IdeaComment> _entityReplyStore;

        public IdeaViewComponent(
            IEntityReplyStore<IdeaComment> entityReplyStore,
            IEntityStore<Idea> entityStore)
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

            var model = await GetViewModel(options);

            return View(model);

        }

        async Task<EntityViewModel<Idea, IdeaComment>> GetViewModel(
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
            return new EntityViewModel<Idea, IdeaComment>
            {
                Options = options,
                Entity = topic
        };

        }

    }

}