using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Articles.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<UserIndex>
    {

        private readonly IAggregatedFeatureEntitiesService _aggregatedFeatureEntitiesService;

        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedFeatureEntitiesService aggregatedFeatureEntitiesService)
        {
            _platoUserStore = platoUserStore;
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedFeatureEntitiesService = aggregatedFeatureEntitiesService;
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
            var indexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Article>)] as EntityIndexViewModel<Article>;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Article>).ToString()} has not been registered on the HttpContext!");
            }

            var featureEntityMetrics = new FeatureEntityMetrics()
            {
                AggregatedResults = await _aggregatedFeatureEntitiesService
                    .ConfigureQuery(q =>
                    {
                        q.CreatedUserId.Equals(user.Id);
                        q.HideSpam.True();
                        q.HideHidden.True();
                        q.HideDeleted.True();
                        q.HidePrivate.True();

                    })
                    .GetResultsAsync()
            };

            // Build view model
            var userDisplayViewModel = new UserDisplayViewModel<Article>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Metrics = featureEntityMetrics
            };

            // Build view
            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => userDisplayViewModel).Zone("header"),
                View<UserDisplayViewModel<Article>>("User.Index.Content", model => userDisplayViewModel).Zone("content"),
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
