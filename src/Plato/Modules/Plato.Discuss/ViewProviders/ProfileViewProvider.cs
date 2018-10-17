using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Discuss.ViewProviders
{
    public class ProfileViewProvider : BaseViewProvider<DiscussUser>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;

        public ProfileViewProvider(
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

            var topicIndexViewModel = context.Controller.HttpContext.Items[typeof(TopicIndexViewModel)] as TopicIndexViewModel;

            return Views(
                View<UserDisplayViewModel>("Profile.Display.Header", model => viewModel).Zone("header"),
                View<TopicIndexViewModel>("Profile.Display.Content", model => topicIndexViewModel).Zone("content")
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
