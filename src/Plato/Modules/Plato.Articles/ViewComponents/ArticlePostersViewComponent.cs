using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Articles.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;

namespace Plato.Articles.ViewComponents
{

    public class ArticlePostersViewComponent : ViewComponent
    {

        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyStore<Comment> _entityReplyStore;
        private readonly IEntityUsersStore _entityUsersStore;

        public ArticlePostersViewComponent(
            IEntityReplyStore<Comment> entityReplyStore,
            IEntityStore<Article> entityStore,
            IEntityUsersStore entityUsersStore)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
            _entityUsersStore = entityUsersStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            
            return View(await GetViewModel(id));

        }

        async Task<TopPostersViewModel> GetViewModel(int id)
        {
            
            // Get top 20 participants
            var users = await _entityUsersStore.QueryAsync()
                .Take(1, 10)
                .Select<EntityUserQueryParams>(q =>
                {
                    q.EntityId.Equals(id);
                })
                .OrderBy("t.TotalReplies", OrderBy.Desc)
                .ToList();

            return new TopPostersViewModel()
            {
                Users = users
            };
        }

    }

}
