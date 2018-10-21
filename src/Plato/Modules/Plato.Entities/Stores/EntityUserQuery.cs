using System;
using System.Collections.Generic;
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
                Params.Keywords.Value
            );

            return data;
        }


    }

    #endregion

    #region "EntityUserQueryParams"

    public class EntityUserQueryParams
    {


        private WhereInt _entityId;
        private WhereString _keywords;


        public WhereInt EntityId
        {
            get => _entityId ?? (_entityId = new WhereInt());
            set => _entityId = value;
        }

        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }


    }

    #endregion

    #region "EntityUserQueryBuilder"

    public class EntityUserQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _entityUsersTableName;

        private readonly EntityUserQuery _query;

        public EntityUserQueryBuilder(EntityUserQuery query)
        {
            _query = query;
            _entityUsersTableName = GetTableNameWithPrefix("EntityUsers");

        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            
            var sb = new StringBuilder();
            sb.Append(BuildTemporaryTable());
            sb.Append(@"           
                SELECT TOP @pageSize
	                u.Id AS UserId,
	                u.UserName,
	                u.NormalizedUserName,
	                u.DisplayName,
	                u.FirstName,
	                u.LastName,
	                u.Alias,
	                r.Id AS LastReplyId,
	                r.CreatedDate AS LastReplyDate,
	                t.TotalReplies
                FROM @t t 
                INNER JOIN {prefix}_Users AS u ON u.Id = t.UserID 
                INNER JOIN {prefix}_EntityReplies AS r ON r.Id = t.LastReplyId              
            ");

            var orderBy = BuildOrderBy();
            sb.Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "Id ASC");
            //sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append(BuildTemporaryTable());
            sb.Append(@"               
                SELECT COUNT(u.Id) FROM @t t 
                INNER JOIN {prefix}_Users AS u ON u.Id = t.UserID 
                INNER JOIN {prefix}_EntityReplies AS r ON r.Id = t.LastReplyId              
            ");
            return sb.ToString();
        }

        string BuildTemporaryTable()
        {
            
            var sb = new StringBuilder();
            sb.Append(@"
                -- temporary table to hold aggregated data
                DECLARE @t TABLE
                (
	                IndexID int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	                UserID int,
	                LastReplyId int,
	                TotalReplies int
                );
                -- insert aggregated data into temporary table
                INSERT INTO @t (UserID, LastReplyId, TotalReplies)
	                SELECT 
	                u.Id AS UserId, 
	                MAX(r.Id) AS LastReplyId,
	                COUNT(r.Id) AS TotalReplies
	                FROM {prefix}_EntityReplies r 
	                JOIN {prefix}_Users u ON r.CreatedUserId = u.Id");
            
            var whereClause = BuildWhereClause();
            if (!string.IsNullOrEmpty(whereClause))
            {
                
                 sb.Append(" WHERE (")
                    .Append(whereClause)
                    .Append(")");
            }

            sb.Append(" GROUP BY u.Id");
        
            return sb.ToString();

        }
        
        #endregion

        #region "Private Methods"

        private string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.TablePrefix)
                ? _query.TablePrefix + tableName
                : tableName;
        }

        private string BuildWhereClause()
        {
            var sb = new StringBuilder();

            // Id
            if (_query.Params.EntityId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("e.EntityId"));
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
