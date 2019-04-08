using System;
using System.Threading.Tasks;
using Plato.Entities.Stores;
using Plato.Ideas.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Ideas.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<UserIndex>
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
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(UserIndex userIndex, IViewProviderContext context)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(userIndex.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userIndex, context);
            }
            
            // Get index view model from context
            var indexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Idea>)] as EntityIndexViewModel<Idea>;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Idea>).ToString()} has not been registered on the HttpContext!");
            }

            // Build view model
            var userDisplayViewModel = new UserDisplayViewModel<Idea>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Metrics = await _featureEntityMetricsStore.GetEntityCountGroupedByFeature(user.Id)
            };
            
            // Build view
            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => userDisplayViewModel).Zone("header"),
                View<UserDisplayViewModel<Idea>>("User.Index.Content", model => userDisplayViewModel).Zone("content"),
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
