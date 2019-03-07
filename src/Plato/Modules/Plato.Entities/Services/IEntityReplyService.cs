using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Entities.ViewModels;

namespace Plato.Entities.Services
{
    public interface IEntityReplyService<TModel> where TModel : class, IEntityReply
    {
        
        Task<IPagedResults<TModel>> GetResultsAsync(EntityOptions options, PagerOptions pager);

        IEntityReplyService<TModel> ConfigureDb(Action<QueryOptions> configure);

        IEntityReplyService<TModel> ConfigureQuery(Action<EntityReplyQueryParams> configure);

    }

}
