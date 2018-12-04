using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Tags.Models;

namespace Plato.Tags.Stores
{

    #region "TagQuery"

    public class TagQuery : DefaultQuery<Tag>
    {

        private readonly IStore<Tag> _store;

        public TagQuery(IStore<Tag> store)
        {
            _store = store;
        }

        public TagQueryParams Params { get; set; }

        public override IQuery<Tag> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (TagQueryParams)Convert.ChangeType(defaultParams, typeof(TagQueryParams));
            return this;
        }

        public override async Task<IPagedResults<Tag>> ToList()
        {

            var builder = new TagQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Keywords.Value
            );

            return data;
        }


    }

    #endregion

    #region "TagQueryParams"

    public class TagQueryParams
    {


        private WhereInt _id;
        private WhereString _keywords;


        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }
        
    }

    #endregion

    #region "TagQueryBuilder"

    public class TagQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _tagsTableName;

        private readonly TagQuery _query;

        public TagQueryBuilder(TagQuery query)
        {
            _query = query;
            _tagsTableName = GetTableNameWithPrefix("Tags");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT ")
                .Append(BuildPopulateSelect())
                .Append(" FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            sb.Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(t.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("t.*");
            return sb.ToString();

        }

        string BuildTables()
        {

            var sb = new StringBuilder();

            sb.Append(_tagsTableName)
                .Append(" t ");

            return sb.ToString();

        }

        #endregion

        #region "Private Methods"

        private string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + tableName
                : tableName;
        }

        private string BuildWhereClause()
        {
            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("t.Id"));
            }

            if (!String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("[Name]", "Keywords"));
            }
            
            return sb.ToString();

        }
        
        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in GetSafeSortColumns())
            {
                sb.Append(sortColumn.Key);
                if (sortColumn.Value != OrderBy.Asc)
                    sb.Append(" DESC");
                if (i < _query.SortColumns.Count - 1)
                    sb.Append(", ");
                i += 1;
            }
            return sb.ToString();
        }

        IDictionary<string, OrderBy> GetSafeSortColumns()
        {
            var ourput = new Dictionary<string, OrderBy>();
            foreach (var sortColumn in _query.SortColumns)
            {
                var columnName = GetSortColumn(sortColumn.Key);
                if (!String.IsNullOrEmpty(columnName))
                {
                    ourput.Add(columnName, sortColumn.Value);
                }
            }
            return ourput;
        }

        string GetSortColumn(string columnName)
        {

            if (String.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            switch (columnName.ToLower())
            {
                case "id":
                    return "Id";
                case "name":
                    return "[Name]";
                case "entities":
                    return "TotalEntities";
                case "totalentities":
                    return "TotalEntities";
                case "follows":
                    return "TotalFollows";
                case "totalfollows":
                    return "TotalFollows";
                case "createddate":
                    return "CreatedDate";
            }

            return string.Empty;

        }


        #endregion
    }

    #endregion

}
