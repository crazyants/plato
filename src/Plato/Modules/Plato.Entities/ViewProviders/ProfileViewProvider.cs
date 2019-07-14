using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Security.Abstractions;

namespace Plato.Entities.ViewProviders
{
    public class ProfileViewProvider : BaseViewProvider<Profile>
    {
        
        private readonly IAggregatedFeatureEntitiesService _aggregatedFeatureEntitiesService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public ProfileViewProvider(
            IAggregatedFeatureEntitiesService aggregatedFeatureEntitiesService,
            IAuthorizationService authorizationService,
            IPlatoUserStore<User> platoUserStore)
        {
            _aggregatedFeatureEntitiesService = aggregatedFeatureEntitiesService;
            _authorizationService = authorizationService;
            _platoUserStore = platoUserStore;
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
            
            var indexOptions = new EntityIndexOptions()
            {
                CreatedByUserId = user.Id
            };




            var featureEntityMetrics = new FeatureEntityMetrics()
            {
                AggregatedResults = await _aggregatedFeatureEntitiesService
                    .ConfigureQuery(async q =>
                    {

                        // Hide private?
                        if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                            Permissions.ViewPrivateEntities))
                        {
                            q.HidePrivate.True();
                        }

                        // Hide hidden?
                        if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                            Permissions.ViewHiddenEntities))
                        {
                            q.HideHidden.True();
                        }

                        // Hide spam?
                        if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                            Permissions.ViewSpamEntities))
                        {
                            q.HideSpam.True();
                        }

                        // Hide deleted?
                        if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                            Permissions.ViewDeletedEntities))
                        {
                            q.HideDeleted.True();
                        }

                    })
                    .GetResultsAsync(indexOptions)
            };
            
            var viewModel = new UserDisplayViewModel<Entity>()
            {
                User = user,
                Metrics = featureEntityMetrics,
                IndexViewModel = new EntityIndexViewModel<Entity>()
                {
                    Options = indexOptions,
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
                    .Order(int.MaxValue - 100)
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
