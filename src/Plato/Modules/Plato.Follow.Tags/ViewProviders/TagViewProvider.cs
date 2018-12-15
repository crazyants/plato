using System.Threading.Tasks;
using Plato.Follow.Stores;
using Plato.Follow.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Follow.Tags.ViewProviders
{
    public class TagViewProvider : BaseViewProvider<Tag>
    {

        private readonly ITagStore<Tag> _tagStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFollowStore<Models.Follow> _followStore;

        public TagViewProvider(
            ITagStore<Tag> tagStore, 
            IContextFacade contextFacade,
            IFollowStore<Models.Follow> followStore)
        {
            _tagStore = tagStore;
            _contextFacade = contextFacade;
            _followStore = followStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Tag tag, IViewProviderContext context)
        {

            var existingTag = await _tagStore.GetByIdAsync(tag.Id);
            if (existingTag == null)
            {
                return await BuildIndexAsync(tag, context);
            }

            var followType = FollowTypes.Tag;
            var isFollowing = false;

            var currentUser = await _contextFacade.GetAuthenticatedUserAsync();
            if (currentUser != null)
            {
                var existingFollow = await _followStore.SelectFollowByNameThingIdAndCreatedUserId(
                    followType.Name,
                    existingTag.Id,
                    currentUser.Id);
                if (existingFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<FollowViewModel>("Follow.Display.Tools", model =>
                {
                    model.FollowType = followType;
                    model.ThingId = existingTag.Id;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("tools").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Tag model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Tag discussUser, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Tag model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
