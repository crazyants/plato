using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Tags.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Tags.ViewModels;
using Plato.Tags.Services;

namespace Plato.Discuss.Tags.ViewComponents
{

    public class DiscussTagListViewComponent : ViewComponent
    {

        private readonly IEnumerable<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Occurrences",
                Value = TagSortBy.Entities
            },
            new SortColumn()
            {
                Text = "Follows",
                Value =  TagSortBy.Follows
            },
            new SortColumn()
            {
                Text = "Created",
                Value =  TagSortBy.Created
            },
            new SortColumn()
            {
                Text = "Last Used",
                Value = TagSortBy.LastEntity
            },
            new SortColumn()
            {
                Text = "Last Modified",
                Value = TagSortBy.Modified
            },
            new SortColumn()
            {
                Text = "Name",
                Value = TagSortBy.Name
            },
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
        
        private readonly ITagService<Tag> _tagService;
        
        public DiscussTagListViewComponent(
            ITagService<Tag> tagService)
        {
            _tagService = tagService;
        }

        public async Task<IViewComponentResult> InvokeAsync(TagIndexOptions options, PagerOptions pager)
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

        async Task<TagIndexViewModel<Tag>> GetViewModel(TagIndexOptions options, PagerOptions pager)
        {

            // Get tags
            var results = await _tagService
                .GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new TagIndexViewModel<Tag>
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
