using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
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
                Text = "Banned",
                Value = FilterBy.Banned
            },
            new Filter()
            {
                Text = "Locked",
                Value = FilterBy.Locked
            },
            new Filter()
            {
                Text = "Spam",
                Value = FilterBy.Spam
            },
            new Filter()
            {
                Text = "Possible Spam",
                Value = FilterBy.PossibleSpam
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

                    switch (options.Filter)
                    {
                        case FilterBy.Confirmed:
                            q.ShowConfirmed.True();
                            break;
                        case FilterBy.Banned:
                            q.ShowBanned.True();
                            break;
                        case FilterBy.Locked:
                            q.ShowLocked.True();
                            break;
                        case FilterBy.Spam:
                            q.ShowSpam.True();
                            break;
                        case FilterBy.PossibleSpam:
                            q.HideConfirmed.True();
                            break;
                        default:
                            break;
                    }

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
                Filters = _defaultFilters,
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }

}

