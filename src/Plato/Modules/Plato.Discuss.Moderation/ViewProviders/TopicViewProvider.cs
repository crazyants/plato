using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.Moderation.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
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

        private readonly IContextFacade _contextFacade;
        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public TopicViewProvider(
            IModeratorStore<Moderator> moderatorStore,
            IPlatoUserStore<User> platoUserStore, 
            IContextFacade contextFacade)
        {
            _moderatorStore = moderatorStore;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Topic topic, IViewProviderContext context)
        {
            
            // Add moderator to context
            await HydrateModeratorContext(topic, context);

            // ----------------

            // Add moderators panel to sidebar

            IPagedResults<User> users = null;

            // Get all moderators
            var moderators = await _moderatorStore
                .QueryAsync()
                .ToList();

            if (moderators != null)
            {
                users = await _platoUserStore.QueryAsync()
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

        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic topic, IViewProviderContext context)
        {

            // Add moderator to context
            await HydrateModeratorContext(topic, context);
            
            return default(IViewProviderResult);

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Topic topic, IViewProviderContext context)
        {

            // Add moderator to context
            await HydrateModeratorContext(topic, context);

            return default(IViewProviderResult);

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IViewProviderContext context)
        {

            // Add moderator to context
            await HydrateModeratorContext(topic, context);

            return default(IViewProviderResult);

        }

        #endregion

        #region "Private Methods"

        async Task HydrateModeratorContext(Topic topic, IViewProviderContext context)
        {

            // Add moderator to context
            if (context.Controller.HttpContext.Items[typeof(Moderator)] == null)
            {
                var user = await _contextFacade.GetAuthenticatedUserAsync();
                if (user != null)
                {
                    context.Controller.HttpContext.Items[typeof(Moderator)] = await GetModerator(user, topic);
                }
            }

        }
        async Task<Moderator> GetModerator(User user, Topic topic)
        {
            
            // Get all moderators
            var moderators = await _moderatorStore
                .QueryAsync()
                .ToList();
            if (moderators == null)
            {
                return null;
            }

            // Get all moderator entries for given identity and resource
            var userEntries = moderators.Data
                .Where(m => m.UserId == user.Id & m.CategoryId == topic.CategoryId)
                .ToList();

            // No moderator entries for the user and resource
            if (!userEntries.Any())
            {
                return null;
            }

            return userEntries[0];

        }


        #endregion

    }


}
