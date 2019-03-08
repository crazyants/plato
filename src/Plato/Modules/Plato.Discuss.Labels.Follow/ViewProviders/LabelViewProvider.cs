using System.Threading.Tasks;
using Plato.Discuss.Labels.Models;
using Plato.Follows.Stores;
using Plato.Follows.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.Follow.ViewProviders
{

    public class LabelViewProvider : BaseViewProvider<Label>
    {

        private readonly ILabelStore<Label> _tagStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;

        public LabelViewProvider(
            ILabelStore<Label> tagStore, 
            IContextFacade contextFacade,
            IFollowStore<Follows.Models.Follow> followStore)
        {
            _tagStore = tagStore;
            _contextFacade = contextFacade;
            _followStore = followStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Label label, IViewProviderContext context)
        {

            var existingTag = await _tagStore.GetByIdAsync(label.Id);
            if (existingTag == null)
            {
                return await BuildIndexAsync(label, context);
            }

            var followType = FollowTypes.Label;
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
                    return model;
                }).Zone("tools").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Label label, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Label label, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Label label, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
