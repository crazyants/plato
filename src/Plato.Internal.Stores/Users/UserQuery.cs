using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
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
            Params = (UserQueryParams) Convert.ChangeType(defaultParams, typeof(UserQueryParams));
            return this;
        }
        
        public override async Task<IPagedResults<User>> ToList()
        {
            var builder = new UserQueryBuilder(this);
            var startSql = builder.BuildSqlStartId();
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            return await _store.SelectAsync(
                PageIndex,
                PageSize,
                startSql,
                populateSql,
                countSql,
                Params.Id.Value,
                Params.UserName.Value,
                Params.Email.Value);
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
            _tableName = !string.IsNullOrEmpty(_query.TablePrefix)
                ? _query.TablePrefix + TableName
                : TableName;
        }

        #endregion

        #region "Implementation"

        public string BuildSqlStartId()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT @start_id_out = Id FROM ").Append(_tableName);
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            if (!string.IsNullOrEmpty(orderBy))
                sb.Append(" ORDER BY ").Append(orderBy);
            return sb.ToString();
        }

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClauseForStartId();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM ").Append(_tableName);
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            if (!string.IsNullOrEmpty(orderBy))
                sb.Append(" ORDER BY ").Append(orderBy);
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

        private string BuildWhereClauseForStartId()
        {
            var sb = new StringBuilder();
            // default to ascending
            if (_query.SortColumns.Count == 0)
                sb.Append("Id >= @start_id_in");
            // set start operator based on first order by
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(sortColumn.Value != OrderBy.Asc
                    ? "Id <= @start_id_in"
                    : "Id >= @start_id_in");
                break;
            }

            var where = BuildWhereClause();
            if (!string.IsNullOrEmpty(where))
                sb.Append(" AND ").Append(where);

            return sb.ToString();

        }
        
        private string BuildWhereClause()
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

            return sb.ToString();
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in _query.SortColumns)
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

        #endregion
    }

    #endregion

}