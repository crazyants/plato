using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Abstractions.Query;

namespace Plato.Stores.Query
{
    public abstract class DefaultQuery : IQuery
    {
        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public IQuery Page(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            return this;
        }

        public abstract IQuery Select<T>(Action<T> configure) where T : new();

        public abstract Task<IEnumerable<T>> ToList<T>() where T : class;
    }
}