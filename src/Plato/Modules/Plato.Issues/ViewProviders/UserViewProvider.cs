using System;
using System.Threading.Tasks;
using Plato.Issues.Models;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Issues.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<UserIndex>
    {

        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IAggregatedEntityRepository aggregatedEntityRepository)
        {
            _platoUserStore = platoUserStore;
            _aggregatedEntityRepository = aggregatedEntityRepository;
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(UserIndex userIndex, IViewProviderContext context)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(userIndex.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userIndex, context);
            }

            // Get index view model from context
            var indexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Issue>)] as EntityIndexViewModel<Issue>;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Issue>).ToString()} has not been registered on the HttpContext!");
            }

            var featureEntityMetrics = new FeatureEntityMetrics()
            {
                Metrics = await _aggregatedEntityRepository.SelectGroupedByFeatureAsync(user.Id)
            };

            // Build view model
            var userDisplayViewModel = new UserDisplayViewModel<Issue>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Metrics = featureEntityMetrics
            };

            // Build view
            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => userDisplayViewModel).Zone("header"),
                View<UserDisplayViewModel<Issue>>("User.Index.Content", model => userDisplayViewModel).Zone("content"),
                View<UserDisplayViewModel>("User.Entities.Display.Sidebar", model => userDisplayViewModel).Zone("sidebar")
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
