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

        private readonly IQueryable<EntityUser> _store;

        public EntityUserQuery(IQueryable<EntityUser> store)
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
