using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{
    public class UserListViewComponent : ViewComponent
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
                Text = "Confirmed",
                Value = FilterBy.Confirmed
            },
            new Filter()
            {
                Text = "Unconfirmed",
                Value = FilterBy.NotConfirmed
            },
            new Filter()
            {
                Text = "Verified",
                Value = FilterBy.Verified
            },
            new Filter()
            {
                Text = "Staff",
                Value = FilterBy.Staff
            },
            new Filter()
            {
                Text = "Spam",
                Value = FilterBy.Spam
            },
            new Filter()
            {
                Text = "Banned",
                Value = FilterBy.Banned
            },
            new Filter()
            {
                Text = "Locked",
                Value = FilterBy.Locked
            }
        };
        
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
                Value =  SortBy.Reputation
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
            },
            new SortColumn()
            {
                Text = "Modified",
                Value = SortBy.ModifiedDate
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
      
        private readonly IUserService<User> _userService;

        public UserListViewComponent(IUserService<User> userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(UserIndexOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new UserIndexOptions();
            }
            
            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetIndexViewModel(options, pager));
        }
        
        private async Task<UserIndexViewModel> GetIndexViewModel(UserIndexOptions options, PagerOptions pager)
        {

            var results = await _userService
                .ConfigureQuery(q =>
                {
                    // We are not within edit mode
                    // Hide spam and banned users
                    if (!options.EnableEdit)
                    {
                        q.HideSpam.True();
                        q.HideBanned.True();
                    }
                })
                .GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new UserIndexViewModel
            {
                SortColumns = _defaultSortColumns,
                SortOrder = _defaultSortOrder,
                Filters = _defaultFilters,
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }

}

