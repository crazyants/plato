using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Categories.Services
{
    public interface ICategoryService<TModel> where TModel : class, ICategory
    {
        Task<IPagedResults<TModel>> GetResultsAsync(CategoryIndexOptions options, PagerOptions pager);

        ICategoryService<TModel> ConfigureDb(Action<IQueryOptions> configure);

        ICategoryService<TModel> ConfigureQuery(Action<CategoryQueryParams> configure);

    }

}
