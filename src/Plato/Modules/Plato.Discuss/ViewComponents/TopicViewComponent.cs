using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Navigation;

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

        public async Task<IViewComponentResult> InvokeAsync(TopicOptions options)
        {

            if (options == null)
            {
                options = new TopicOptions();
            }

            var model = await GetViewModel(options);

            return View(model);

        }

        async Task<TopicViewModel> GetViewModel(
            TopicOptions options)
        {

            if (options.Params.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Params.EntityId));
            }

            var topic = await _entityStore.GetByIdAsync(options.Params.EntityId);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }
            
            // Return view model
            return new TopicViewModel
            {
                Options = options,
                Topic = topic
        };

        }

    }

}
