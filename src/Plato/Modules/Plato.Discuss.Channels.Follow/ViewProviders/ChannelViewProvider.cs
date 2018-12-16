using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Follow.Stores;
using Plato.Follow.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Channels.Follow.ViewProviders
{

    public class ChannelViewProvider : BaseViewProvider<Channel>
    {

        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFollowStore<Plato.Follow.Models.Follow> _followStore;

        public ChannelViewProvider(
            IContextFacade contextFacade,
            IFollowStore<Plato.Follow.Models.Follow> followStore,
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

            if (channel.Id == 0)
            {
                return default(IViewProviderResult);
            }

            var existingChannel = await _channelStore.GetByIdAsync(channel.Id);
            if (existingChannel == null)
            {
                return default(IViewProviderResult);
            }

            var followType = FollowTypes.Channel;
            var isFollowing = false;

            var currentUser = await _contextFacade.GetAuthenticatedUserAsync();
            if (currentUser != null)
            {
                var existingFollow = await _followStore.SelectFollowByNameThingIdAndCreatedUserId(
                    followType.Name,
                    existingChannel.Id,
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
                    model.ThingId = existingChannel.Id;
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
