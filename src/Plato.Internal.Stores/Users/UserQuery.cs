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
                Params.Keywords.Value);
        }

    }

    #endregion

    #region "UserQueryParams"

    public class UserQueryParams
    {

        private WhereInt _id;
        private WhereString _keywords;
        private WhereInt _roleId;
        private WhereString _roleName;
        private WhereBool _showSpam;
        private WhereBool _hideSpam;
        private WhereBool _showBanned;
        private WhereBool _hideBanned;
        private WhereBool _showConfirmed;
        private WhereBool _hideConfirmed;
        private WhereBool _showLocked;
        private WhereBool _hideLocked;
        private WhereEnum<UserType> _userType;
        
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

        public WhereBool HideSpam
        {
            get => _hideSpam ?? (_hideSpam = new WhereBool());
            set => _hideSpam = value;
        }

        public WhereBool ShowSpam
        {
            get => _showSpam ?? (_showSpam = new WhereBool());
            set => _showSpam = value;
        }

        public WhereBool HideBanned
        {
            get => _hideBanned ?? (_hideBanned = new WhereBool());
            set => _hideBanned = value;
        }

        public WhereBool ShowBanned
        {
            get => _showBanned ?? (_showBanned = new WhereBool());
            set => _showBanned = value;
        }

        public WhereBool HideConfirmed
        {
            get => _hideConfirmed ?? (_hideConfirmed = new WhereBool());
            set => _hideConfirmed = value;
        }

        public WhereBool ShowConfirmed
        {
            get => _showConfirmed ?? (_showConfirmed = new WhereBool());
            set => _showConfirmed = value;
        }


        public WhereBool HideLocked
        {
            get => _hideLocked ?? (_hideLocked = new WhereBool());
            set => _hideLocked = value;
        }

        public WhereBool ShowLocked
        {
            get => _showLocked ?? (_showLocked = new WhereBool());
            set => _showLocked = value;
        }

        public WhereEnum<UserType> UserType
        {
            get => _userType ?? (_userType = new WhereEnum<UserType>());
            set => _userType = value;
        }
    }

    #endregion

    #region "UserQueryBuilder"

    public class UserQueryBuilder : IQueryBuilder
    {

        #region "Constructor"

        private readonly string _usersTableName;
        private readonly UserQuery _query;

        public UserQueryBuilder(UserQuery query)
        {
            _query = query;
            _usersTableName = GetTableNameWithPrefix("Users");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM ").Append(_usersTableName);
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            sb.Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "LastLoginDate DESC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(Id) FROM ").Append(_usersTableName);
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + tableName
                : tableName;
        }

        string BuildWhereClause()
        {

            var sb = new StringBuilder();

            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("Id"));
            }

            if (!String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb
                    .Append(_query.Params.Keywords.ToSqlString("UserName", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("DisplayName", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("Email", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("FirstName", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("LastName", "Keywords"));
            }

            // -----------------
            // UserType 
            // -----------------

            if (_query.Params.UserType.Value != UserType.None)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserType.Operator);
                sb.Append(_query.Params.UserType.ToSqlString("UserType"));

            }

            // -----------------
            // RoleName 
            // -----------------

            if (!String.IsNullOrEmpty(_query.Params.RoleName.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.RoleName.Operator);
                sb.Append(_query.Params.RoleName.ToSqlString("RoleName", "Keywords"));
                
            }

            // -----------------
            // IsSpam 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideSpam.Value && !_query.Params.ShowSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideSpam.Operator);
                sb.Append("IsSpam = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowSpam.Value && !_query.Params.HideSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowSpam.Operator);
                sb.Append("IsSpam = 1");
            }


            // -----------------
            // IsBanned 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideBanned.Value && !_query.Params.ShowBanned.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideBanned.Operator);
                sb.Append("IsBanned = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowBanned.Value && !_query.Params.HideBanned.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowBanned.Operator);
                sb.Append("IsBanned = 1");
            }

            // -----------------
            // EmailConfirmed 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideConfirmed.Value && !_query.Params.ShowConfirmed.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideConfirmed.Operator);
                sb.Append("EmailConfirmed = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowConfirmed.Value && !_query.Params.HideConfirmed.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowConfirmed.Operator);
                sb.Append("EmailConfirmed = 1");
            }

            // -----------------
            // LockoutEnabled 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideLocked.Value && !_query.Params.ShowLocked.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideLocked.Operator);
                sb.Append("LockoutEnabled = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowLocked.Value && !_query.Params.HideLocked.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowLocked.Operator);
                sb.Append("LockoutEnabled = 1");
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
                case "visits":
                    return "Visits";
                case "totalvisits":
                    return "Visits";
                case "minutes":
                    return "MinutesActive";
                case "minutesactive":
                    return "MinutesActive";
                case "reputation":
                    return "Reputation";
                case "totalreputation":
                    return "Reputation";
                case "rank":
                    return "[Rank]";
                case "createddate":
                    return "CreatedDate";
                case "modified":
                    return "ModifiedDate";
                case "modifieddate":
                    return "ModifiedDate";
                case "lastlogindate":
                    return "LastLoginDate";
            }

            return string.Empty;
            
        }

        #endregion

    }

    #endregion

}