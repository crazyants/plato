using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Categories.Models;
using Plato.Follows.Models;
using Plato.Follows.Stores;
using Plato.Follows.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Categories.Follow.ViewProviders
{

    public class ChannelViewProvider : BaseViewProvider<Category>
    {

        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;

        public ChannelViewProvider(
            IContextFacade contextFacade,
            IFollowStore<Follows.Models.Follow> followStore,
            ICategoryStore<Category> categoryStore)
        {
            _contextFacade = contextFacade;
            _followStore = followStore;
            _categoryStore = categoryStore;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Category categoryAdmin, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Category categoryAdmin, IViewProviderContext context)
        {

            if (categoryAdmin == null)
            {
                return default(IViewProviderResult);
            }
            
            // Get follow type
            FollowType followType = null;
            followType = categoryAdmin.Id == 0
                ? FollowTypes.AllChannels
                : FollowTypes.Channel;
            
            // Get thingId if available
            var thingId = 0;
            if (categoryAdmin.Id > 0)
            {
                var existingChannel = await _categoryStore.GetByIdAsync(categoryAdmin.Id);
                if (existingChannel != null)
                {
                    thingId = existingChannel.Id;
                }
            }

            // Are we already following?
            var isFollowing = false;
            var currentUser = await _contextFacade.GetAuthenticatedUserAsync();
            if (currentUser != null)
            {
                var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                    followType.Name,
                    thingId,
                    currentUser.Id);
                if (existingFollow != null)
                {
                    isFollowing = true;
                }
            }

            return Views(
                View<FollowViewModel>("Follow.Display.Sidebar", model =>
                {
                    model.FollowType = followType;
                    model.ThingId = thingId;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("sidebar").Order(2)
            );


        }

        public override Task<IViewProviderResult> BuildEditAsync(Category categoryAdmin, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Category categoryAdmin, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
