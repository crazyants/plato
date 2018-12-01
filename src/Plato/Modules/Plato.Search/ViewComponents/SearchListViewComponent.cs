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


        private readonly IList<Filter> _defaultFilters = new List<Filter>()
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

        private readonly IList<SortColumn> _defaultSortColumns = new List<SortColumn>()
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

        private readonly IList<SortOrder> _defaultSortOrder = new List<SortOrder>()
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

        private SearchSettings _searchSettings;

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

            // Get search settings
            _searchSettings = await _searchSettingsStore.GetAsync();
            
            // Get view model
            var model = await GetIndexViewModel(options, pager);

            // If full text is enabled add rank to sort options
            if (_searchSettings != null)
            {
                if (_searchSettings.SearchType != SearchTypes.Tsql)
                {
                    model.SortColumns.Insert(0, new SortColumn()
                    {
                        Text = "Relevency",
                        Value = SortBy.Rank
                    });
                }

            }

            // Return view model
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
            SearchIndexOptions options,
            PagerOptions pagerOptions)
        {

   
            return await _entityStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Configure(opts =>
                {
                    if (_searchSettings != null)
                    {
                        opts.SearchType = _searchSettings.SearchType;
                    }
                })
                .Select<EntityQueryParams>(q =>
                {
               
                    if (options.ChannelId > 0)
                    {
                        q.CategoryId.Equals(options.ChannelId);
                    }

                    if (!string.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }
                    
                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();
               
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();
        }
        
    }
    
}

