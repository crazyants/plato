using System.Threading.Tasks;
using Plato.Discuss.Tags.Models;
using Plato.Follows.Stores;
using Plato.Follows.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.Follow.ViewProviders
{
    public class TagViewProvider : BaseViewProvider<Tag>
    {
    
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IContextFacade _contextFacade;
        private readonly ITagStore<TagBase> _tagStore;

        public TagViewProvider(
            IFollowStore<Follows.Models.Follow> followStore,
            IContextFacade contextFacade,
            ITagStore<TagBase> tagStore)
        {
            _contextFacade = contextFacade;
            _followStore = followStore;
            _tagStore = tagStore;
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
                var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
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
                    model.Permission = Permissions.FollowDiscussTags;
                    return model;
                }).Zone("tools").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Tag model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Tag user, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Tag model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
