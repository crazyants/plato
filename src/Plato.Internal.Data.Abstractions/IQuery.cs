using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IQuery
    {

        string TablePrefix { get; set; }

        IQuery Page(int pageIndex, int pageSize);

        IQuery Select<T>(Action<T> configure) where T : new();

        IQuery OrderBy(string columnName, OrderBy sortOrder = Abstractions.OrderBy.Asc);

        Task<IPagedResults<T>> ToList<T>() where T : class;

        IDictionary<string, OrderBy> SortColumns { get; }

    }



    public interface IQuery<TModel> where TModel : class
    {

        string TablePrefix { get; set; }

        IQuery<TModel> Page(int pageIndex, int pageSize);

        IQuery<TModel> Select<T>(Action<T> configure) where T : new();

        IQuery<TModel> OrderBy(string columnName, OrderBy sortOrder = Abstractions.OrderBy.Asc);

        Task<IPagedResults<TModel>> ToList();

        IDictionary<string, OrderBy> SortColumns { get; }

    }


    public enum OrderBy
    {
        Desc = 0,
        Asc = 1
    }

}