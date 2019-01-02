using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.Services
{

    public interface IUserService
    {
        Task<IPagedResults<User>> GetUsersAsunc(UserIndexOptions options, PagerOptions pager);

    }


    public class UserService : IUserService
    {

        private readonly IPlatoUserStore<User> _ploatUserStore;

        public UserService(IPlatoUserStore<User> ploatUserStore)
        {
            _ploatUserStore = ploatUserStore;
        }

        public async Task<IPagedResults<User>> GetUsersAsunc(
            UserIndexOptions options,
            PagerOptions pager)
        {
            return await _ploatUserStore.QueryAsync()
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

        }

    }

}
