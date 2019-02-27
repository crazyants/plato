using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Discuss.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<DiscussUser>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(DiscussUser discussUser, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(discussUser.Id);
            if (user == null)
            {
                return await BuildIndexAsync(discussUser, context);
            }

            var viewModel = new UserDisplayViewModel()
            {
                User = user
            };

            var topicIndexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;

            return Views(
                View<UserDisplayViewModel>("User.Discuss.Display.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Topic>>("User.Discuss.Display.Content", model => topicIndexViewModel).Zone("content")
            );


        }

        public override Task<IViewProviderResult> BuildIndexAsync(DiscussUser model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(DiscussUser discussUser, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(DiscussUser model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
