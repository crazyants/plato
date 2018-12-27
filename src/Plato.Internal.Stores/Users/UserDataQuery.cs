using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Users
{

    #region "UserDataQuery"

    public class UserDataQuery : DefaultQuery<UserData>
    {

        private readonly IStore<UserData> _store;

        public UserDataQuery(IStore<UserData> store)
        {
            _store = store;
        }

        public UserDataQueryParams Params { get; set; }

        public override IQuery<UserData> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (UserDataQueryParams)Convert.ChangeType(defaultParams, typeof(UserDataQueryParams));
            return this;
        }

        public override async Task<IPagedResults<UserData>> ToList()
        {

            var builder = new UserDataQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Key.Value
            );

            return data;
        }
        
    }

    #endregion

    #region "UserDataQueryParams"

    public class UserDataQueryParams
    {

        private WhereInt _id;
        private WhereInt _userId;
        private WhereString _key;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt UserId
        {
            get => _userId ?? (_userId = new WhereInt());
            set => _userId = value;
        }

        public WhereString Key
        {
            get => _key ?? (_key = new WhereString());
            set => _key = value;
        }

    }

    #endregion

    #region "UserDataQueryBuilder"

    public class UserDataQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _userDataTableName;

        private readonly UserDataQuery _query;

        public UserDataQueryBuilder(UserDataQuery query)
        {
            _query = query;
            _userDataTableName = GetTableNameWithPrefix("UserData");
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
            sb.Append("SELECT COUNT(d.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }
        
        #endregion

        #region "Private Methods"

        string BuildPopulateSelect()
        {
            return "*";

        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_userDataTableName).Append(" d ");
            return sb.ToString();
        }

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
                sb.Append(_query.Params.Id.ToSqlString("d.Id"));
            }

            // UserId
            if (_query.Params.UserId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserId.Operator);
                sb.Append(_query.Params.UserId.ToSqlString("UserId"));
            }

            // Keywords
            if (!string.IsNullOrEmpty(_query.Params.Key.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Key.Operator);
                sb.Append(_query.Params.Key.ToSqlString("Key", "Key"));
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
                : "d." + columnName;
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
