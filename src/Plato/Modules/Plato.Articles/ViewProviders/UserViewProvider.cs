using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Articles.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Articles.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<ArticlesUser>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(ArticlesUser articlesUser, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(articlesUser.Id);
            if (user == null)
            {
                return await BuildIndexAsync(articlesUser, context);
            }

            var viewModel = new UserDisplayViewModel()
            {
                User = user
            };

            var topicIndexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] as EntityIndexViewModel<Entity>;

            return Views(
                View<UserDisplayViewModel>("User.Articles.Display.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Entity>>("User.Articles.Display.Content", model => topicIndexViewModel).Zone("content")
            );


        }

        public override Task<IViewProviderResult> BuildIndexAsync(ArticlesUser model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(ArticlesUser articlesUser, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ArticlesUser model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
