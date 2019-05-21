using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Models;
using Plato.Discuss.SimilarTopics.ViewModels;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Search.Models;
using Plato.Search.Stores;


namespace Plato.Discuss.SimilarTopics.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityService<Topic> _entityService;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IFeatureFacade _featureFacade;

        public TopicViewProvider(
            ISearchSettingsStore<SearchSettings> searchSettingsStore,
            IAuthorizationService authorizationService,
            IEntityService<Topic> entityService, 
            IEntityStore<Topic> entityStore,
            IFeatureFacade featureFacade)
        {
            _entityService = entityService;
            _authorizationService = authorizationService;
            _entityStore = entityStore;
            _searchSettingsStore = searchSettingsStore;
            _featureFacade = featureFacade;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Topic topic, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic topic, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityViewModel<Topic, Reply>)] as EntityViewModel<Topic, Reply>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityViewModel<Topic, Reply>).ToString()} has not been registered on the HttpContext!");
            }
            
            // Get the entity we are viewing 
            var entity = await _entityStore.GetByIdAsync(topic.Id);

            // Ensure we found the entity
            if (entity == null)
            {
                return default(IViewProviderResult);
            }

            // Get search settings
            var searchSettings = await _searchSettingsStore.GetAsync();

            // Build keywords to search
            var keywordList = entity.Title
                .ToDistinctList()
                .StripCommonWords()
                .ToList();
            
            // Get similar entities
            var entities = await _entityService
                .ConfigureDb(o =>
                {
                    if (searchSettings != null)
                    {
                        o.SearchType = searchSettings.SearchType;
                    }
                })
                .ConfigureQuery(async q =>
                {

                    // Hide current entity
                    q.Id.NotEqual(entity.Id);

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                        Permissions.ViewPrivateTopics))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                        Permissions.ViewHiddenTopics))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                        Permissions.ViewSpamTopics))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                        Permissions.ViewDeletedTopics))
                    {
                        q.HideDeleted.True();
                    }

                    // Search for similar entities
                    if (keywordList.Count > 0)
                    {
                        var keywords = String.Join(" ", keywordList.ToArray());
                        q.Keywords.Like(keywords);
                    }

                })
                .GetResultsAsync(
                    new EntityIndexOptions()
                    {
                        FeatureId = entity.FeatureId
                    }, new PagerOptions()
                    {
                        Page = 1,
                        Size = 10
                    });

            // Build similar topics view model
            var similarEntitiesViewModel = new SimilarEntitiesViewModel()
            {
                Results = entities
            };

            // Return view
            return Views(
                View<SimilarEntitiesViewModel>("Topic.Similar.Display.Sidebar", model => similarEntitiesViewModel).Zone("sidebar").Order(6)
            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Topic topic, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
               
        public override Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }

}
