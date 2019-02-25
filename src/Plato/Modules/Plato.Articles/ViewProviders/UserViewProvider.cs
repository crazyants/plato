using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Articles.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<UserProfile>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(UserProfile userProfile, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, context);
            }

            //var viewModel = new UserDisplayViewModel()
            //{
            //    User = user
            //};
            
            var topicIndexViewModel = new EntityIndexViewModel<Article>()
            {
                Options = new EntityIndexOptions()
                { 
                    //EnableCard = false,
                    CreatedByUserId = user.Id
                },
                Pager = new PagerOptions()
                {
                    Page = 1,
                    PageSize = 5,
                    Enabled = false
                }
            };

            return Views(
                    View<EntityIndexViewModel<Article>>("Articles.Profile.Display.Content", model => topicIndexViewModel).Zone("content")
                );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(UserProfile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserProfile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserProfile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
