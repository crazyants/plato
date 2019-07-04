using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Articles.Tags.ViewModels;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Articles.Tags.ViewComponents
{

    public class ArticleTagsViewComponent : ViewComponent
    {

        private readonly IEntityTagStore<EntityTag> _tagStore;

        public ArticleTagsViewComponent(IEntityTagStore<EntityTag> tagStore)
        {
            _tagStore = tagStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(Article entity, Comment reply)
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
