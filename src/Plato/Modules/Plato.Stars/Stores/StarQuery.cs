using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Stars.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Stars.Stores
{

    #region "FollowQuery"

    public class StarQuery : DefaultQuery<Star>
    {

        private readonly IStore<Star> _store;

        public StarQuery(IStore<Star> store)
        {
            _store = store;
        }

        public FollowQueryParams Params { get; set; }

        public override IQuery<Star> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (FollowQueryParams)Convert.ChangeType(defaultParams, typeof(FollowQueryParams));
            return this;
        }

        public override async Task<IPagedResults<Star>> ToList()
        {

            var builder = new FollowQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Name.Value
            );

            return data;
        }
        
    }

    #endregion

    #region "FollowQueryParams"

    public class FollowQueryParams
    {
        
        private WhereInt _id;
        private WhereInt _thingId;
        private WhereString _name;
      
        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt ThingId
        {
            get => _thingId ?? (_thingId = new WhereInt());
            set => _thingId = value;
        }
        
        public WhereString Name
        {
            get => _name ?? (_name = new WhereString());
            set => _name = value;
        }
        
    }

    #endregion

    #region "FollowQueryBuilder"

    public class FollowQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _followsTableName;
        private readonly string _usersTableName;

        private readonly StarQuery _query;

        public FollowQueryBuilder(StarQuery query)
        {
            _query = query;
            _followsTableName = GetTableNameWithPrefix("Follows");
            _usersTableName = GetTableNameWithPrefix("Users");

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
                    : "f.Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(f.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("f.*, ")
                .Append("u.Email, ")
                .Append("u.UserName, ")
                .Append("u.DisplayName, ")
                .Append("u.NormalizedUserName");
            return sb.ToString();

        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_followsTableName)
                .Append(" f WITH (nolock) LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" u ON f.CreatedUserId = u.Id");
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
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("f.Id"));
            }

            // ThingId
            if (_query.Params.ThingId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ThingId.Operator);
                sb.Append(_query.Params.ThingId.ToSqlString("f.ThingId"));
            }

            // Name
            if (!String.IsNullOrEmpty(_query.Params.Name.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Name.Operator);
                sb.Append(_query.Params.Name.ToSqlString("f.[Name]", "Name"));
            }
            return sb.ToString();

        }
        
        string GetQualifiedColumnName(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            return columnName.IndexOf('.') >= 0
                ? columnName
                : "f." + columnName;
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(GetQualifiedColumnName(sortColumn.Key));
                if (sortColumn.Value != OrderBy.Asc)
                    sb.Append(" DESC");
                if (i < _query.SortColumns.Count - 1)
                    sb.Append(", ");
                i += 1;
            }
            return sb.ToString();
        }

        #endregion

    }

    #endregion

}
