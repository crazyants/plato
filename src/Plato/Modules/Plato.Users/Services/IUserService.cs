using System;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.Services
{

    public interface IUserService<TModel> where TModel : class, IUser
    {

        IUserService<TModel> ConfigureDb(Action<IQueryOptions> configure);

        Task<IPagedResults<TModel>> GetResultsAsync(UserIndexOptions options, PagerOptions pager);

        IUserService<TModel> ConfigureQuery(Action<UserQueryParams> configure);

    }

}
