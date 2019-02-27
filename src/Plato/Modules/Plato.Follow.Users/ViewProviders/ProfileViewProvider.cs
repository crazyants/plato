using System.Threading.Tasks;
using Plato.Follows.Stores;
using Plato.Follows.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Follow.Users.ViewProviders
{
    public class ProfileViewProvider : BaseViewProvider<Profile>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;

        public ProfileViewProvider(
            IPlatoUserStore<User> platoUserStore, 
            IContextFacade contextFacade,
            IFollowStore<Follows.Models.Follow> followStore)
        {
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _followStore = followStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Profile discuss, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(discuss.Id);
            if (user == null)
            {
                return await BuildIndexAsync(discuss, context);
            }

            var followType = FollowTypes.User;
            var isFollowing = false;

            var currentUser = await _contextFacade.GetAuthenticatedUserAsync();
            if (currentUser != null)
            {
                var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                    followType.Name,
                    user.Id,
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
                    model.ThingId = user.Id;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("tools").Order(1)
            );


        }

        public override Task<IViewProviderResult> BuildIndexAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Profile discuss, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
