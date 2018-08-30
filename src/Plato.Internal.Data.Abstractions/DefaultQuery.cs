using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{

    public abstract class DefaultQuery<TModel> : IQuery<TModel> where TModel : class
    {

        private readonly Dictionary<string, OrderBy> _sortColumns;

        public IDictionary<string, OrderBy> SortColumns => _sortColumns;

        public int PageIndex { get; private set; } = 1;

        public int PageSize { get; private set; } = int.MaxValue;

        public string TablePrefix { get; set; }

        public IQuery<TModel> Take(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            return this;
        }

        public abstract IQuery<TModel> Select<T>(Action<T> configure) where T : new();
        
        public abstract Task<IPagedResults<TModel>> ToList();

        public IQuery<TModel> OrderBy(string columnName, OrderBy sortOrder = Abstractions.OrderBy.Asc)
        {
            _sortColumns.Add(columnName, sortOrder);
            return this;
        }

        protected DefaultQuery()
        {
            _sortColumns = new Dictionary<string, OrderBy>();
        }

    }

}