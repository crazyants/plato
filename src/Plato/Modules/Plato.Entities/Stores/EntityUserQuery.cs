using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    #region "EntityUserQuery"

    public class EntityUserQuery : DefaultQuery<EntityUser>
    {

        private readonly IQueryableStore<EntityUser> _store;

        public EntityUserQuery(IQueryableStore<EntityUser> store)
        {
            _store = store;
        }

        public EntityUserQueryParams Params { get; set; }

        public override IQuery<EntityUser> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityUserQueryParams)Convert.ChangeType(defaultParams, typeof(EntityUserQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EntityUser>> ToList()
        {

            var builder = new EntityUserQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Username.Value
            );

            return data;
        }


    }

    #endregion

    #region "EntityUserQueryParams"

    public class EntityUserQueryParams
    {
        
        private WhereInt _entityId;
        private WhereString _userName;
        private WhereBool _showHidden;
        private WhereBool _hideHidden;
        private WhereBool _showSpam;
        private WhereBool _hideSpam;
        private WhereBool _hideDeleted;
        private WhereBool _showDeleted;

        public WhereInt EntityId
        {
            get => _entityId ?? (_entityId = new WhereInt());
            set => _entityId = value;
        }

        public WhereString Username
        {
            get => _userName ?? (_userName = new WhereString());
            set => _userName = value;
        }
        
        public WhereBool ShowHidden
        {
            get => _showHidden ?? (_showHidden = new WhereBool());
            set => _showHidden = value;
        }

        public WhereBool HideHidden
        {
            get => _hideHidden ?? (_hideHidden = new WhereBool());
            set => _hideHidden = value;
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
        
        public WhereBool HideDeleted
        {
            get => _hideDeleted ?? (_hideDeleted = new WhereBool());
            set => _hideDeleted = value;
        }

        public WhereBool ShowDeleted
        {
            get => _showDeleted ?? (_showDeleted = new WhereBool());
            set => _showDeleted = value;
        }

    }

    #endregion

    #region "EntityUserQueryBuilder"

    public class EntityUserQueryBuilder : IQueryBuilder
    {

        #region "Constructor"

        private readonly string _usersTableName;
        private readonly string _repliesTableName;
        
        private readonly EntityUserQuery _query;

        public EntityUserQueryBuilder(EntityUserQuery query)
        {
            _query = query;
            _usersTableName = GetTableNameWithPrefix("Users");
            _repliesTableName = GetTableNameWithPrefix("EntityReplies");

        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {

            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();

            sb.Append(BuildTemporaryTable())
                .Append("SELECT ")
                .Append("u.Id AS UserId, ")
                .Append("u.UserName, ")
                .Append("u.NormalizedUserName, ")
                .Append("u.DisplayName,")
                .Append("u.Alias, ")
                .Append("u.PhotoUrl, ")
                .Append("u.PhotoColor, ")
                .Append("r.Id AS LastReplyId, ")
                .Append("r.CreatedDate AS LastReplyDate, ")
                .Append("t.TotalReplies ")
                .Append("FROM @t t ")
                .Append("INNER JOIN ")
                .Append(_usersTableName)
                .Append(" AS u ON u.Id = t.UserID ")
                .Append("INNER JOIN ")
                .Append(_repliesTableName)
                .Append(" AS r ON r.Id = t.LastReplyId")
                .Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "Id ASC")
                .Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append(BuildTemporaryTable());
            sb.Append("SELECT COUNT(u.Id) FROM @t t ")
                .Append("INNER JOIN ")
                .Append(_usersTableName)
                .Append(" AS u ON u.Id = t.UserID ")
                .Append(" INNER JOIN ")
                .Append(_repliesTableName)
                .Append(" AS r ON r.Id = t.LastReplyId");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        string BuildTemporaryTable()
        {

            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();

            sb.Append("DECLARE @t TABLE")
                .Append("(")
                .Append("IndexID int IDENTITY (1, 1) NOT NULL PRIMARY KEY,")
                .Append("UserID int,")
                .Append("LastReplyId int,")
                .Append("TotalReplies int")
                .Append(");");

            sb.Append("INSERT INTO @t (UserID, LastReplyId, TotalReplies) ")
                .Append("SELECT ")
                .Append("u.Id AS UserId, ")
                .Append("MAX(r.Id) AS LastReplyId, ")
                .Append("COUNT(r.Id) AS TotalReplies ")
                .Append("FROM ")
                .Append(_repliesTableName)
                .Append(" r JOIN ")
                .Append(_usersTableName)
                .Append(" u ON r.CreatedUserId = u.Id");
            if (!string.IsNullOrEmpty(whereClause))
            {
                 sb.Append(" WHERE (")
                    .Append(whereClause)
                    .Append(")");
            }
            sb.Append(" GROUP BY u.Id;");
        
            return sb.ToString();

        }
        
        string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + tableName
                : tableName;
        }

        string BuildWhereClause()
        {
            var sb = new StringBuilder();

            // EntityId
            if (_query.Params.EntityId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("r.EntityId"));
            }

            // Username
            if (!string.IsNullOrEmpty(_query.Params.Username.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.Username.ToSqlString("Username", "Keywords"));
            }

            // -----------------
            // IsHidden 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideHidden.Value && !_query.Params.ShowHidden.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideHidden.Operator);
                sb.Append("r.IsHidden = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowHidden.Value && !_query.Params.HideHidden.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowHidden.Operator);
                sb.Append("r.IsHidden = 1");
            }

            // -----------------
            // IsSpam 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideSpam.Value && !_query.Params.ShowSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideSpam.Operator);
                sb.Append("r.IsSpam = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowSpam.Value && !_query.Params.HideSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowSpam.Operator);
                sb.Append("r.IsSpam = 1");
            }

            // -----------------
            // IsDeleted 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideDeleted.Value && !_query.Params.ShowDeleted.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideDeleted.Operator);
                sb.Append("r.IsDeleted = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowDeleted.Value && !_query.Params.HideDeleted.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowDeleted.Operator);
                sb.Append("r.IsDeleted = 1");
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
                : "e." + columnName;
        }

        string BuildOrderBy()
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
