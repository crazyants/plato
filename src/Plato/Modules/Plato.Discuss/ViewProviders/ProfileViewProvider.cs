using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Discuss.ViewProviders
{
    public class ProfileViewProvider : BaseViewProvider<Profile>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public ProfileViewProvider(
            IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Profile profile, IViewProviderContext context)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(profile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(profile, context);
            }
            
            // Build view model
            var viewModel = new EntityIndexViewModel<Topic>()
            {
                Options = new EntityIndexOptions()
                { 
                    CreatedByUserId = user.Id
                },
                Pager = new PagerOptions()
                {
                    Page = 1,
                    PageSize = 5,
                    Enabled = false
                }
            };

            // Return view
            return Views(
                View<EntityIndexViewModel<Topic>>("Profile.Discuss.Display.Content", model => viewModel)
                    .Zone("content")
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
