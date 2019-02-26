using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;

namespace Plato.Discuss.ViewComponents
{

    public class TopicViewComponent : ViewComponent
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;

        public TopicViewComponent(
            IEntityReplyStore<Reply> entityReplyStore,
            IEntityStore<Topic> entityStore)
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
        async Task<EntityViewModel<Topic, Reply>> GetViewModel(
            EntityOptions options)
        {

            if (options.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.EntityId));
            }

            var topic = await _entityStore.GetByIdAsync(options.EntityId);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }

            // Return view model
            return new EntityViewModel<Topic, Reply>
            {
                Options = options,
                Entity = topic
            };

        }

    }

}
