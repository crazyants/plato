using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Docs.Tags.ViewModels;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Docs.Tags.ViewComponents
{

    public class DocTagsViewComponent : ViewComponent
    {

        private readonly IEntityTagStore<EntityTag> _tagStore;

        public DocTagsViewComponent(IEntityTagStore<EntityTag> tagStore)
        {
            _tagStore = tagStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(Doc entity, DocComment reply)
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
