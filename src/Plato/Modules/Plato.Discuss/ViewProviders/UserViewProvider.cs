using System;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Discuss.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<UserIndex>
    {

        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore, 
            IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository)
        {
            _platoUserStore = platoUserStore;
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(UserIndex userIndex,
            IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(userIndex.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userIndex, context);
            }

            var indexViewModel =
                context.Controller.HttpContext.Items[
                    typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (indexViewModel == null)
            {
                throw new Exception(
                    $"A view model of type {typeof(EntityIndexViewModel<Topic>).ToString()} has not been registered on the HttpContext!");
            }

            var featureEntityMetrics = new FeatureEntityMetrics()
            {
                Metrics = await _aggregatedEntityMetricsRepository.SelectGroupedByFeature(user.Id)
            };
            
            var userDisplayViewModel = new UserDisplayViewModel<Topic>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Metrics = featureEntityMetrics
            };

            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => userDisplayViewModel).Zone("header"),
                View<UserDisplayViewModel<Topic>>("User.Index.Content", model => userDisplayViewModel).Zone("content"),
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
