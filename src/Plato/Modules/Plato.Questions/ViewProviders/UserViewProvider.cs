using System;
using System.Threading.Tasks;
using Plato.Questions.Models;
using Plato.Questions.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Questions.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<UserIndex>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(UserIndex userIndex, IViewProviderContext context)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(userIndex.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userIndex, context);
            }

            // Build view model
            var viewModel = new UserDisplayViewModel()
            {
                User = user
            };

            // Get index view model from context
            var indexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Question>)] as EntityIndexViewModel<Question>;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Question>).ToString()} has not been registered on the HttpContext!");
            }
            
            // Build view
            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Question>>("User.Index.Content", model => indexViewModel).Zone("content")
            );
            
        }

        public override Task<IViewProviderResult> BuildIndexAsync(UserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserIndex userIndex, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
