using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Labels.Services;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Labels.ViewComponents
{

    public class LabelListViewComponent : ViewComponent
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
        
        private readonly ILabelService _labelService;

        public LabelListViewComponent(
            ILabelService labelService)
        {
            _labelService = labelService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            LabelIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new LabelIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }

        async Task<LabelIndexViewModel> GetViewModel(
            LabelIndexOptions options,
            PagerOptions pager)
        {

            var results = await _labelService.GetLabelsAsunc(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new LabelIndexViewModel
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
