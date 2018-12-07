using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.ViewComponents
{

    public class TagListViewComponent : ViewComponent
    {

        private readonly IEntityTagStore<EntityTag> _tagStore;

        public TagListViewComponent(
            IEntityTagStore<EntityTag> tagStore)
        {
            _tagStore = tagStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            Topic topic,
            Reply reply)
        {

            // Get all tags for entity
            var tags = await _tagStore.GetByEntityId(topic.Id);
            
            return View(new TagListViewModel()
            {
                Topic = topic,
                Reply = reply,
                Tags = tags
            });

        }

    }

}
