using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Ideas.Tags.ViewModels;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Ideas.Tags.ViewComponents
{

    public class IdeaTagsViewComponent : ViewComponent
    {

        private readonly IEntityTagStore<EntityTag> _tagStore;

        public IdeaTagsViewComponent(IEntityTagStore<EntityTag> tagStore)
        {
            _tagStore = tagStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(Idea entity, IdeaComment reply)
        {

            // We always need a entity to display tags
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Get tags and return view
            var tags = await _tagStore.GetByEntityIdAsync(entity.Id);
            return View(new TagListViewModel()
            {
                Topic = entity,
                Reply = reply,
                Tags = tags?
                    .Where(t => t.EntityReplyId == (reply?.Id ?? 0))
                    .OrderByDescending(t => t.TotalEntities)
            });

        }

    }

}
