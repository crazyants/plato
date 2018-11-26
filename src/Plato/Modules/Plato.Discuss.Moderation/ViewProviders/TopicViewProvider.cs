using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.Moderation.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Discuss.Moderation.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly IPlatoUserStore<User> _ploatUserStore;

        public TopicViewProvider(
            IModeratorStore<Moderator> moderatorStore,
            IPlatoUserStore<User> ploatUserStore)
        {
            _moderatorStore = moderatorStore;
            _ploatUserStore = ploatUserStore;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Topic moderator, IViewProviderContext updater)
        {

            IPagedResults<User> users = null;

            // Get all moderators
            var moderators = await _moderatorStore
                .QueryAsync()
                .ToList();

            if (moderators != null)
            {
                users = await _ploatUserStore.QueryAsync()
                    .Take(1, 20)
                    .Select<UserQueryParams>(q =>
                    {
                        q.Id.IsIn(moderators.Data.Select(m => m.UserId).ToArray());
                    })
                    .OrderBy("LastLoginDate", OrderBy.Desc)
                    .ToList();
            }
            
            return Views(View<ModeratorsViewModel>("Topic.Moderators.Index.Sidebar", model =>
                {
                    model.Moderators = users?.Data ?? null;
                    return model;
                }).Zone("sidebar").Order(100)
            );
            
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Topic oldModerator, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildEditAsync(Topic moderator, IViewProviderContext updater)
        {

            return Task.FromResult(default(IViewProviderResult));
        }


        public override Task<IViewProviderResult> BuildUpdateAsync(Topic model, IViewProviderContext context)
        {

            return Task.FromResult(default(IViewProviderResult));

        }

        #endregion
        

    }


}
