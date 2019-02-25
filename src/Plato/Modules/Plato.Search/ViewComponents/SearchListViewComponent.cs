using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Search.Models;
using Plato.Search.Services;
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
        
        private readonly ISearchService _searchService;

        public SearchListViewComponent(
            ISearchService searchService,
            ISearchSettingsStore<SearchSettings> searchSettingsStore)
        {
            _searchService = searchService;
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

            // Get view model
            var model = await GetIndexViewModel(options, pager);

            // If full text is enabled add rank to sort options
            if (options.Sort == SortBy.Rank)
            {
                model.SortColumns.Insert(0, new SortColumn()
                {
                    Text = "Relevancy",
                    Value = SortBy.Rank
                });
            }

            // Return view model
            return View(model);

        }
        
        async Task<SearchIndexViewModel> GetIndexViewModel(
            SearchIndexOptions options,
            PagerOptions pager)
        {

            // Build results
            var results = await _searchService.GetResultsAsync(options, pager); // GetEntities(options, pager);

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
    
    }
    
}

