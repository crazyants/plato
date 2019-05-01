using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;

namespace Plato.Issues.ViewComponents
{

    public class IssueViewComponent : ViewComponent
    {

        private readonly IEntityStore<Issue> _entityStore;
        private readonly IEntityReplyStore<Comment> _entityReplyStore;

        public IssueViewComponent(
            IEntityReplyStore<Comment> entityReplyStore,
            IEntityStore<Issue> entityStore)
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

        async Task<EntityViewModel<Issue, Comment>> GetViewModel(
            EntityOptions options)
        {

            if (options.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Id));
            }

            var entity = await _entityStore.GetByIdAsync(options.Id);
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            // Return view model
            return new EntityViewModel<Issue, Comment>
            {
                Options = options,
                Entity = entity
            };

        }

    }

}
  