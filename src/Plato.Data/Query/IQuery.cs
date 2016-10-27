using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Query
{
    public interface IQuery
    {

        int PageIndex { get; set; }

        int PageSize { get; set; }

        ICriteria Criteria { get; set; }

        string SortBy { get; set; }

        QuerySortOrder SortOrder { get; set; }

        string BuildSql();

        string BuildSqlCount();

    }

    public enum QuerySortOrder
    {
        Asc,
        Desc
    }

}
