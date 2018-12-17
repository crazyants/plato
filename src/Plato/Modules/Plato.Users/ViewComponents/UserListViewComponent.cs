using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{
    public class UserListViewComponent : ViewComponent
    {


        private readonly IEnumerable<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Last Active",
                Value = SortBy.LastLoginDate
            },
            new SortColumn()
            {
                Text = "Reputation",
                Value =  SortBy.Points
            },
            new SortColumn()
            {
                Text = "Rank",
                Value = SortBy.Rank
            },
            new SortColumn()
            {
                Text = "Time Spent",
                Value =  SortBy.Minutes
            },
            new SortColumn()
            {
                Text = "Visits",
                Value = SortBy.Visits
            },
            new SortColumn()
            {
                Text = "Created",
                Value = SortBy.CreatedDate
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
        private readonly IPlatoUserStore<User> _ploatUserStore;

        public UserListViewComponent(
            IContextFacade contextFacade,
            IPlatoUserStore<User> ploatUserStore)
        {
            _contextFacade = contextFacade;
            _ploatUserStore = ploatUserStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            UserIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new UserIndexOptions();
            }
            
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            var model = await GetIndexViewModel(options, pager);
            return View(model);
        }
        
        private async Task<UserIndexViewModel> GetIndexViewModel(
            UserIndexOptions options,
            PagerOptions pager)
        {

            var results = await _ploatUserStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<UserQueryParams>(q =>
                {
                    if (!string.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();
            
            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new UserIndexViewModel
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

