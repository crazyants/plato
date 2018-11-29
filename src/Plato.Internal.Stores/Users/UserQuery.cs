using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Stores.Users
{

    #region "UserQuery"

    public class UserQuery : DefaultQuery<User>
    {

        private readonly IStore<User> _store;

        public UserQuery(IStore<User> store)
        {
            _store = store;
        }

        public UserQueryParams Params { get; set; }

        public override IQuery<User> Select<TParams>(Action<TParams> configure)
        {
            var defaultParams = new TParams();
            configure(defaultParams);
            Params = (UserQueryParams)Convert.ChangeType(defaultParams, typeof(UserQueryParams));
            return this;
        }

        public override async Task<IPagedResults<User>> ToList()
        {
            var builder = new UserQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            return await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Id.Value,
                Params.UserName.Value,
                Params.Email.Value,
                Params.DisplayName.Value,
                Params.FirstName.Value,
                Params.LastName.Value);
        }

    }

    #endregion

    #region "UserQueryParams"

    public class UserQueryParams
    {

        private WhereInt _id;
        private WhereString _email;
        private WhereString _userName;
        private WhereString _firstName;
        private WhereString _lastName;
        private WhereString _displayName;
        private WhereInt _roleId;
        private WhereString _roleName;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereString UserName
        {
            get => _userName ?? (_userName = new WhereString());
            set => _userName = value;
        }

        public WhereString FirstName
        {
            get => _firstName ?? (_firstName = new WhereString());
            set => _firstName = value;
        }

        public WhereString LastName
        {
            get => _lastName ?? (_lastName = new WhereString());
            set => _lastName = value;
        }

        public WhereString DisplayName
        {
            get => _displayName ?? (_displayName = new WhereString());
            set => _displayName = value;
        }

        public WhereString Email
        {
            get => _email ?? (_email = new WhereString());
            set => _email = value;
        }
        
        public WhereInt RoleId
        {
            get => _roleId ?? (_roleId = new WhereInt());
            set => _roleId = value;
        }

        public WhereString RoleName
        {
            get => _roleName ?? (_roleName = new WhereString());
            set => _roleName = value;
        }

    }

    #endregion

    #region "UserQueryBuilder"

    public class UserQueryBuilder : IQueryBuilder
    {

        #region "Constructor"

        private readonly string _tableName;
        private const string TableName = "Users";

        private readonly UserQuery _query;

        public UserQueryBuilder(UserQuery query)
        {
            _query = query;
            _tableName = !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + TableName
                : TableName;
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM ").Append(_tableName);
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
            sb.Append("SELECT COUNT(Id) FROM ").Append(_tableName);
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        string BuildWhereClause()
        {
            var sb = new StringBuilder();

            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("Id"));
            }

            if (!String.IsNullOrEmpty(_query.Params.UserName.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserName.Operator);
                sb.Append(_query.Params.UserName.ToSqlString("UserName"));
            }
            
            if (!String.IsNullOrEmpty(_query.Params.Email.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Email.Operator);
                sb.Append(_query.Params.Email.ToSqlString("Email"));
            }

            if (!String.IsNullOrEmpty(_query.Params.FirstName.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.FirstName.Operator);
                sb.Append(_query.Params.FirstName.ToSqlString("FirstName"));
            }

            if (!String.IsNullOrEmpty(_query.Params.LastName.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.LastName.Operator);
                sb.Append(_query.Params.LastName.ToSqlString("LastName"));
            }

            return sb.ToString();
        }

        string BuildOrderBy()
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
                case "username":
                    return "UserName";
                case "email":
                    return "Email";
                case "firstname":
                    return "FirstName";
                case "lastname":
                    return "LastName";
                case "createddate":
                    return "CreatedDate";
                case "lastlogindate":
                    return "LastLoginDate";
            }

            return string.Empty;
            
        }

        #endregion

    }

    #endregion

}