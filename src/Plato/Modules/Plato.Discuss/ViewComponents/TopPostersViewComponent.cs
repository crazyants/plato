using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewComponents
{

    public class TopPostersViewComponent : ViewComponent
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IEntityUsersStore _entityUsersStore;

        public TopPostersViewComponent(
            IEntityReplyStore<Reply> entityReplyStore,
            IEntityStore<Topic> entityStore,
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
                .Take(1, 20)
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
