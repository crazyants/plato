using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Tags.Services;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Tags.ViewComponents
{

    public class TagListViewComponent : ViewComponent
    {

        private readonly IEnumerable<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Popular",
                Value = SortBy.Entities
            },
            new SortColumn()
            {
                Text = "Follows",
                Value =  SortBy.Follows
            },
            new SortColumn()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortColumn()
            {
                Text = "First Use",
                Value =  SortBy.Created
            },
            new SortColumn()
            {
                Text = "Last Use",
                Value = SortBy.LastEntity
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


        private readonly ITagService _tagService;
        
        public TagListViewComponent(
            ITagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            TagIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new TagIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }

        async Task<TagIndexViewModel> GetViewModel(
            TagIndexOptions options,
            PagerOptions pager)
        {

            // Get tags
            var results = await _tagService.GetTagsAsunc(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new TagIndexViewModel
            {
                SortColumns = _defaultSortColumns,
                SortOrder = _defaultSortOrder,
                Results = results,
                Options = options,
                Pager = pager
            };
            
        }

    }

}
