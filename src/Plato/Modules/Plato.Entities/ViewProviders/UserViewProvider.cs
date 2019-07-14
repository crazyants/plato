using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Entities.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<EntityUserIndex>
    {

        private readonly IAggregatedFeatureEntitiesService _aggregatedFeatureEntitiesService;
        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IPlatoUserStore<User> _platoUserStore;
  

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedFeatureEntitiesService aggregatedFeatureEntitiesService,
            IAuthorizationService authorizationService)
        {
            _platoUserStore = platoUserStore;
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedFeatureEntitiesService = aggregatedFeatureEntitiesService;
            _authorizationService = authorizationService;
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
                    .GetResultsAsync()
            };

            var userDisplayViewModel = new UserDisplayViewModel<Entity>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Metrics = featureEntityMetrics
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
