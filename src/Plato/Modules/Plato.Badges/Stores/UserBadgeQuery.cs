using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Badges.Stores
{

    #region "UserBadgeQuery"

    public class UserBadgeQuery : DefaultQuery<UserBadge>
    {

        private readonly IStore<UserBadge> _store;

        public UserBadgeQuery(IStore<UserBadge> store)
        {
            _store = store;
        }

        public UserBadgeQueryParams Params { get; set; }

        public override IQuery<UserBadge> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (UserBadgeQueryParams)Convert.ChangeType(defaultParams, typeof(UserBadgeQueryParams));
            return this;
        }

        public override async Task<IPagedResults<UserBadge>> ToList()
        {

            var builder = new UserBadgeQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var badgeName = Params?.BadgeName?.Value ?? string.Empty;

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                badgeName
            );

            return data;
        }
        
    }

    #endregion

    #region "UserBadgeQueryParams"

    public class UserBadgeQueryParams
    {

        private WhereInt _id;
        private WhereString _badgeName;
        private WhereInt _userId;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereString BadgeName
        {
            get => _badgeName ?? (_badgeName = new WhereString());
            set => _badgeName = value;
        }

        public WhereInt UserId
        {
            get => _userId ?? (_userId = new WhereInt());
            set => _userId = value;
        }

    }

    #endregion

    #region "UserBadgeQueryBuilder"

    public class UserBadgeQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _userBadgesTableName;
  
        private readonly UserBadgeQuery _query;

        public UserBadgeQueryBuilder(UserBadgeQuery query)
        {
            _query = query;
            _userBadgesTableName = GetTableNameWithPrefix("UserBadges");
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
            sb.Append("SELECT COUNT(ub.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("ub.*");
            return sb.ToString();
        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_userBadgesTableName)
                .Append(" ub ");
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

            if (_query.Params == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("ub.Id"));
            }
            
            // BadgeName
            if (!String.IsNullOrEmpty(_query.Params.BadgeName.Value) )
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.BadgeName.Operator);
                sb.Append(_query.Params.BadgeName.ToSqlString("BadgeName"));
            }

            // UserId
            if (_query.Params.UserId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.UserId.ToSqlString("UserId"));
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
                : "ub." + columnName;
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
