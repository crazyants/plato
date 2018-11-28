using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Search.Models;
using Plato.Search.Stores;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;

        public SearchListViewComponent(
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore,
            IFeatureFacade featureFacade,
            ISearchSettingsStore<SearchSettings> searchSettingsStore)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _featureFacade = featureFacade;
            _searchSettingsStore = searchSettingsStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            SearchIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new SearchIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            var model = await GetIndexViewModel(options, pager);

            return View(model);

        }
        
        async Task<SearchIndexViewModel> GetIndexViewModel(
            SearchIndexOptions searchIndexOptions,
            PagerOptions pagerOptions)
        {
            var results = await GetEntities(searchIndexOptions, pagerOptions);
            return new SearchIndexViewModel(
                results,
                searchIndexOptions,
                pagerOptions);
        }


        async Task<IPagedResults<Entity>> GetEntities(
            SearchIndexOptions searchIndexOpts,
            PagerOptions pagerOptions)
        {

            // Explictly get Plato.Discuss feature, this view component can be 
            // used in different areas (i.e. Plat.Discuss.Channels) so use explict area name
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");

            var searchSettings =  await _searchSettingsStore.GetAsync();
            
            return await _entityStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Configure(options =>
                {
                    options.SearchType = searchSettings.SearchType;
                })
                .Select<EntityQueryParams>(q =>
                {
                    
                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

                    if (searchIndexOpts.ChannelId > 0)
                    {
                        q.CategoryId.Equals(searchIndexOpts.ChannelId);
                    }

                    switch (searchSettings.SearchType)
                    {

                        case SearchTypes.ContainsTable:

                            if (!string.IsNullOrEmpty(searchIndexOpts.Search))
                            {
                                q.Title.ContainsTable(searchIndexOpts.Search).Or();
                                q.Message.ContainsTable(searchIndexOpts.Search).Or();
                                q.Html.ContainsTable(searchIndexOpts.Search).Or();
                            }

                            break;

                        case SearchTypes.FreeTextTable:

                            if (!string.IsNullOrEmpty(searchIndexOpts.Search))
                            {
                                q.Title.FreeTextTable(searchIndexOpts.Search).Or();
                                q.Message.FreeTextTable(searchIndexOpts.Search).Or();
                                q.Html.FreeTextTable(searchIndexOpts.Search).Or();
                            }

                            break;

                        default:

                            if (!string.IsNullOrEmpty(searchIndexOpts.Search))
                            {
                                q.Title.Like(searchIndexOpts.Search).Or();
                                q.Message.Like(searchIndexOpts.Search).Or();
                                q.Html.Like(searchIndexOpts.Search).Or();
                            }

                            break;
                    }



                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();
               
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
        }
        

    }


}

