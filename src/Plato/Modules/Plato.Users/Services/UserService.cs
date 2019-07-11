using System;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.Services
{
    
    public class UserService<TModel> : IUserService<TModel> where TModel : class, IUser
    {

        private Action<QueryOptions> _configureDb = null;
        private Action<UserQueryParams> _configureParams = null;

        private readonly IPlatoUserStore<TModel> _platoUserStore;

        public UserService(IPlatoUserStore<TModel> platoUserStore)
        {
            _platoUserStore = platoUserStore;

            // Default options delegate
            _configureDb = options => options.SearchType = SearchTypes.Tsql;

        }

        public IUserService<TModel> ConfigureDb(Action<IQueryOptions> configure)
        {
            _configureDb = configure;
            return this;
        }

        public IUserService<TModel> ConfigureQuery(Action<UserQueryParams> configure)
        {
            _configureParams = configure;
            return this;
        }
        
        public async Task<IPagedResults<TModel>> GetResultsAsync(
            UserIndexOptions options,
            PagerOptions pager)
        {
            return await _platoUserStore.QueryAsync()
                .Configure(_configureDb)
                .Take(pager.Page, pager.Size)
                .Select<UserQueryParams>(q =>
                {

                    switch (options.Filter)
                    {
                        case FilterBy.Confirmed:
                            q.ShowConfirmed.True();
                            break;
                        case FilterBy.Unconfirmed:
                            q.HideConfirmed.True();
                            break;
                        case FilterBy.Verified:
                            q.ShowVerified.True();
                            break;
                        case FilterBy.Staff:
                            q.ShowStaff.True();
                            break;
                        case FilterBy.Spam:
                            q.ShowSpam.True();
                            break;
                        case FilterBy.Banned:
                            q.ShowBanned.True();
                            break;
                        case FilterBy.Locked:
                            q.ShowLocked.True();
                            break;
                    }

                    if (!string.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }
                    
                    // ----------------
                    // Additional parameter configuration
                    // ----------------

                    _configureParams?.Invoke(q);

                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

        }

    }

}
