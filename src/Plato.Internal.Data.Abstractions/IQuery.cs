using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    
    public interface IQuery<TModel> where TModel : class
    {

        IQueryOptions Options { get; }
        
        IQuery<TModel> Take(int page, int size);

        IQuery<TModel> Take(int size);

        IQuery<TModel> Configure(Action<QueryOptions> configure);

        IQuery<TModel> Select<TParams>(Action<TParams> configure) where TParams : new();

        IQuery<TModel> OrderBy(string columnName, OrderBy sortOrder = Abstractions.OrderBy.Asc);

        IQuery<TModel> OrderBy(IDictionary<string, OrderBy> columns);
        
        Task<IPagedResults<TModel>> ToList();

        IDictionary<string, OrderBy> SortColumns { get; }

    }
    
    public enum OrderBy
    {
        Desc = 0,
        Asc = 1
    }

}