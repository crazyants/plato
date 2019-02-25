using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Articles.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Services
{

    public interface ITopicService
    {

        Task<IPagedResults<Article>> GetResultsAsync(TopicIndexOptions options, PagerOptions pager);

    }

}
