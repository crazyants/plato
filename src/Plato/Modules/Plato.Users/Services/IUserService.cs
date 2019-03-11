using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Users.ViewModels;

namespace Plato.Users.Services
{

    public interface IUserService
    {
        Task<IPagedResults<User>> GetResultsAsync(UserIndexOptions options, PagerOptions pager);
    }

}
