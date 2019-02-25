using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Articles.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Services
{

    public interface IArticleService
    {

        Task<IPagedResults<Article>> GetResultsAsync(ArticleIndexOptions options, PagerOptions pager);

    }

}
