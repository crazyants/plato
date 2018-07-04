using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public abstract class DefaultQuery<TModel> : IQuery where TModel : class
    {

        private readonly Dictionary<string, OrderBy> _sortColumns;

        public IDictionary<string, OrderBy> SortColumns => _sortColumns;

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public string TablePrefix { get; set; }

        public IQuery Page(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            return this;
        }

        public abstract IQuery Select<T>(Action<T> configure) where T : new();

        public abstract Task<IPagedResults<T>> ToList<T>() where T : class;

        public abstract Task<IPagedResults<TModel>> ToList();
        
        public IQuery OrderBy(string columnName, OrderBy sortOrder = Abstractions.OrderBy.Asc)
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