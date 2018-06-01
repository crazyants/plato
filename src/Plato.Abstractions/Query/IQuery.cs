using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Abstractions.Collections;

namespace Plato.Abstractions.Query
{
    public interface IQuery
    {

        string TablePrefix { get; }

        IQuery Page(int pageIndex, int pageSize);

        IQuery Select<T>(Action<T> configure) where T : new();

        IQuery OrderBy(string columnName, OrderBy sortOrder = Query.OrderBy.Asc);

        Task<IPagedResults<T>> ToList<T>() where T : class;

        IDictionary<string, Query.OrderBy> SortColumns { get; }

    }

    public enum OrderBy
    {
        Desc = 0,
        Asc = 1
    }

}