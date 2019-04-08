using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Entities.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<EntityUserIndex>
    {

        private readonly IFeatureEntityMetricsStore _featureEntityMetricsStore;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IFeatureEntityMetricsStore featureEntityMetricsStore)
        {
            _platoUserStore = platoUserStore;
            _featureEntityMetricsStore = featureEntityMetricsStore;
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(EntityUserIndex userIndex, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(userIndex.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userIndex, context);
            }

            var indexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] as EntityIndexViewModel<Entity>;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Entity>).ToString()} has not been registered on the HttpContext!");
            }

            var userDisplayViewModel = new UserDisplayViewModel<Entity>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Metrics = await _featureEntityMetricsStore.GetEntityCountGroupedByFeature(user.Id)
            };
            
            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => userDisplayViewModel).Zone("header"),
                View<UserDisplayViewModel<Entity>>("User.Index.Content", model => userDisplayViewModel).Zone("content"),
                View< UserDisplayViewModel>("User.Entities.Display.Sidebar", model => userDisplayViewModel).Zone("sidebar")
            );
            
        }

        public override Task<IViewProviderResult> BuildIndexAsync(EntityUserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(EntityUserIndex userIndex, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(EntityUserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
