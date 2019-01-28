using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Follows.Models;
using Plato.Follows.Stores;
using Plato.Follows.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Channels.Follow.ViewProviders
{

    public class ChannelViewProvider : BaseViewProvider<Channel>
    {

        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFollowStore<Plato.Follows.Models.Follow> _followStore;

        public ChannelViewProvider(
            IContextFacade contextFacade,
            IFollowStore<Plato.Follows.Models.Follow> followStore,
            ICategoryStore<Channel> channelStore)
        {
            _contextFacade = contextFacade;
            _followStore = followStore;
            _channelStore = channelStore;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Channel channel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Channel channel, IViewProviderContext context)
        {

            if (channel == null)
            {
                return default(IViewProviderResult);
            }
            
            // Get follow type
            FollowType followType = null;
            followType = channel.Id == 0
                ? FollowTypes.AllChannels
                : FollowTypes.Channel;
            
            // Get thingId if available
            var thingId = 0;
            if (channel.Id > 0)
            {
                var existingChannel = await _channelStore.GetByIdAsync(channel.Id);
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

        public override Task<IViewProviderResult> BuildEditAsync(Channel channel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Channel channel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
