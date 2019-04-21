using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Entities.ViewProviders
{
    public class ProfileViewProvider : BaseViewProvider<Profile>
    {

        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;

        //private readonly IFeatureEntityMetricsStore _featureEntityMetricsStore;
        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public ProfileViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository)
        {
            _platoUserStore = platoUserStore;
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Profile profile, IViewProviderContext context)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(profile.Id);

            // Ensure user exists
            if (user == null)
            {
                return await BuildIndexAsync(profile, context);
            }


            var featureEntityMetrics = new FeatureEntityMetrics()
            {
                Metrics = await _aggregatedEntityMetricsRepository.SelectGroupedByFeature(user.Id)
            };
            
            var viewModel = new UserDisplayViewModel<Entity>()
            {
                User = user,
                Metrics = featureEntityMetrics,
                IndexViewModel = new EntityIndexViewModel<Entity>()
                {
                    Options = new EntityIndexOptions()
                    {
                        CreatedByUserId = user.Id
                    },
                    Pager = new PagerOptions()
                    {
                        Page = 1,
                        Size = 10,
                        Enabled = false
                    }
                }
            };
         
            // Return view
            return Views(
                View<UserDisplayViewModel<Entity>>("Profile.Entities.Display.Content", model => viewModel)
                    .Zone("content")
                    .Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
