using System.Collections.Generic;
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


        private readonly IEnumerable<Filter> _defaultFilters = new List<Filter>()
        {
            new Filter()
            {
                Text = "All",
                Value = FilterBy.All
            },
            new Filter()
            {
                Text = "-" // represents menu divider
            },
            new Filter()
            {
                Text = "My Topics",
                Value = FilterBy.MyTopics
            },
            new Filter()
            {
                Text = "Participated",
                Value = FilterBy.Participated
            },
            new Filter()
            {
                Text = "Following",
                Value = FilterBy.Following
            },
            new Filter()
            {
                Text = "Starred",
                Value = FilterBy.Starred
            },
            new Filter()
            {
                Text = "-"  // represents menu divider
            },
            new Filter()
            {
                Text = "Unanswered",
                Value = FilterBy.Unanswered
            },
            new Filter()
            {
                Text = "No Replies",
                Value = FilterBy.NoReplies
            }
        };

        private readonly IEnumerable<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Last Reply",
                Value = SortBy.LastReply
            },
            new SortColumn()
            {
                Text = "Replies",
                Value =  SortBy.Replies
            },
            new SortColumn()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortColumn()
            {
                Text = "Participants",
                Value =  SortBy.Participants
            },
            new SortColumn()
            {
                Text = "Reactions",
                Value =  SortBy.Reactions
            },
            new SortColumn()
            {
                Text = "Created",
                Value = SortBy.Created
            },
            new SortColumn()
            {
                Text = "Modified",
                Value = SortBy.Modified
            }
        };

        private readonly IEnumerable<SortOrder> _defaultSortOrder = new List<SortOrder>()
        {
            new SortOrder()
            {
                Text = "Descending",
                Value = OrderBy.Desc
            },
            new SortOrder()
            {
                Text = "Ascending",
                Value = OrderBy.Asc
            },
        };
        
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
            SearchIndexOptions options,
            PagerOptions pager)
        {

            // Build results
            var results = await GetEntities(options, pager);

            // Set pager total
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new SearchIndexViewModel
            {
                Results = results,
                Options = options,
                Pager = pager,
                SortColumns = _defaultSortColumns,
                SortOrder = _defaultSortOrder,
                Filters = _defaultFilters
            };
        }


        async Task<IPagedResults<Entity>> GetEntities(
            SearchIndexOptions searchIndexOpts,
            PagerOptions pagerOptions)
        {

            // Explictly get Plato.Discuss feature, this view component can be 
            // used in different areas (i.e. Plat.Discuss.Channels) so use explict area name
            //var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");

            // Get search settings
            var searchSettings =  await _searchSettingsStore.GetAsync();
            
            return await _entityStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Configure(options =>
                {
                    if (searchSettings != null)
                    {
                        options.SearchType = searchSettings.SearchType;
                    }
                })
                .Select<EntityQueryParams>(q =>
                {
                    
                    //if (feature != null)
                    //{
                    //    q.FeatureId.Equals(feature.Id);
                    //}

                    if (searchIndexOpts.ChannelId > 0)
                    {
                        q.CategoryId.Equals(searchIndexOpts.ChannelId);
                    }

                    if (!string.IsNullOrEmpty(searchIndexOpts.Search))
                    {
                        q.Keywords.Like(searchIndexOpts.Search);
                    }
                    
                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();
               
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
        }
        

    }


}

