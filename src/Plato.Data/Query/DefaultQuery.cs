using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Query
{
    public abstract class DefaultQuery : IQuery
    {
        public virtual ICriteria Criteria { get; set; }

        public virtual int PageIndex { get; set; } = 1;

        public virtual int PageSize { get; set; } = 1;

        public virtual string SortBy { get; set; } = "Id";

        public virtual QuerySortOrder SortOrder { get; set; } = QuerySortOrder.Desc;

        public abstract string BuildSql();

        public abstract string BuildSqlCount();

    }
}
