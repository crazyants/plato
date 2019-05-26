using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Notifications.Models;

namespace Plato.Notifications.Stores
{

    #region "UserNotificationsQuery"

    public class UserNotificationsQuery : DefaultQuery<UserNotification>
    {

        private readonly IStore2<UserNotification> _store;

        public UserNotificationsQuery(IStore2<UserNotification> store)
        {
            _store = store;
        }

        public UserNotificationsQueryParams Params { get; set; }

        public override IQuery<UserNotification> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (UserNotificationsQueryParams)Convert.ChangeType(defaultParams, typeof(UserNotificationsQueryParams));
            return this;
        }

        public override async Task<IPagedResults<UserNotification>> ToList()
        {

            var builder = new UserNotificationsQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var notificationName = Params.NotificationName.Value ?? string.Empty;

            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("NotificationName", DbType.String, notificationName)
            });

        }

    }

    #endregion

    #region "UserNotificationsQueryParams"

    public class UserNotificationsQueryParams
    {

        private WhereInt _userId;
        private WhereString _notificationName;
        private WhereBool _showRead;
        private WhereBool _hideread;

        public WhereInt UserId
        {
            get => _userId ?? (_userId = new WhereInt());
            set => _userId = value;
        }
        
        public WhereString NotificationName
        {
            get => _notificationName ?? (_notificationName = new WhereString());
            set => _notificationName = value;
        }

        public WhereBool ShowRead
        {
            get => _showRead ?? (_showRead = new WhereBool());
            set => _showRead = value;
        }

        public WhereBool HideRead
        {
            get => _hideread ?? (_hideread = new WhereBool());
            set => _hideread = value;
        }

    }

    #endregion

    #region "UserNotificationsQueryBuilder"

    public class UserNotificationsQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _userNotificationsTableName;
        private readonly string _usersTableName;

        private readonly UserNotificationsQuery _query;

        public UserNotificationsQueryBuilder(UserNotificationsQuery query)
        {
            _query = query;
            _userNotificationsTableName = GetTableNameWithPrefix("UserNotifications");
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
                    : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(un.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("un.*,")
                .Append("u.UserName,")
                .Append("u.DisplayName,")
                .Append("u.Alias,")
                .Append("u.PhotoUrl,")
                .Append("u.PhotoColor,")
                .Append("c.UserName AS CreatedUserName,")
                .Append("c.DisplayName AS CreatedDisplayName,")
                .Append("c.Alias AS CreatedAlias,")
                .Append("c.PhotoUrl AS CreatedPhotoUrl,")
                .Append("c.PhotoColor AS CreatedPhotoColor");
            return sb.ToString();

        }

        string BuildTables()
        {

            var sb = new StringBuilder();
            sb.Append(_userNotificationsTableName)
                .Append(" un WITH (nolock) LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" u ON un.UserId = u.Id LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" c ON un.CreatedUserId = c.Id");

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

            // UserId
            if (_query.Params.UserId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserId.Operator);
                sb.Append(_query.Params.UserId.ToSqlString("un.UserId"));
            }
            
            if (!string.IsNullOrEmpty(_query.Params.NotificationName.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.NotificationName.Operator);
                sb.Append(_query.Params.NotificationName.ToSqlString("un.NotificationName", "Keywords"));
            }

            // -----------------
            // read / unread 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideRead.Value && !_query.Params.ShowRead.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideRead.Operator);
                sb.Append("un.ReadDate IS NULL");
            }

            // show = true, hide = false
            if (_query.Params.ShowRead.Value && !_query.Params.HideRead.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowRead.Operator);
                sb.Append("un.ReadDate IS NOT NULL");
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
                : "un." + columnName;
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
