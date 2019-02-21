using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Users
{
    #region "UserRoleQuery"

    public class UserRoleQuery : DefaultQuery<UserRole>
    {

        private readonly IStore<UserRole> _store;

        public UserRoleQuery(IStore<UserRole> store)
        {
            _store = store;
        }

        public UserRoleQueryParams Params { get; set; }

        public override IQuery<UserRole> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (UserRoleQueryParams)Convert.ChangeType(defaultParams, typeof(UserRoleQueryParams));
            return this;
        }

        public override async Task<IPagedResults<UserRole>> ToList()
        {

            var builder = new UserRoleQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Keywords.Value,
                Params.RoleName.Value
            );

            return data;
        }

    }

    #endregion

    #region "UserRoleQueryParams"

    public class UserRoleQueryParams
    {

        private WhereInt _id;
        private WhereInt _userId;
        private WhereString _keywords;
        private WhereString _roleName;

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

        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }

        public WhereString RoleName
        {
            get => _roleName ?? (_roleName = new WhereString());
            set => _roleName = value;
        }


    }

    #endregion

    #region "UserRoleQueryBuilder"

    public class UserRoleQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _rolesTableName;
        private readonly string _userRolesTableName;

        private readonly UserRoleQuery _query;

        public UserRoleQueryBuilder(UserRoleQuery query)
        {
            _query = query;
            _rolesTableName = GetTableNameWithPrefix("Roles");
            _userRolesTableName = GetTableNameWithPrefix("UserRoles");
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
            sb.Append("SELECT COUNT(ur.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("ur.Id, ur.UserId, ")
                .Append("r.Id AS RoleId, ")
                .Append("r.[Name], ")
                .Append("r.NormalizedName, ")
                .Append("r.Description, ")
                .Append("r.Claims, ")
                .Append("r.CreatedDate, ")
                .Append("r.CreatedUserId, ")
                .Append("r.ModifiedDate, ")
                .Append("r.ModifiedUserId, ")
                .Append("r.ConcurrencyStamp");

            return sb.ToString();

        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_userRolesTableName)
                .Append(" ur INNER JOIN ")
                .Append(_rolesTableName)
                .Append(" r ON ur.RoleId = r.Id")
                .Append("");
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
                sb.Append(_query.Params.Id.ToSqlString("ur.Id"));
            }

            // UserId
            if (_query.Params.UserId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserId.Operator);
                sb.Append(_query.Params.UserId.ToSqlString("ur.UserId"));
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
