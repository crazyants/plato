using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.History.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.History.Stores
{

    #region "EntityHistoryQuery"

    public class EntityHistoryQuery : DefaultQuery<EntityHistory>
    {

        private readonly IStore<EntityHistory> _store;

        public EntityHistoryQuery(IStore<EntityHistory> store)
        {
            _store = store;
        }

        public EntityHistoryQueryParams Params { get; set; }

        public override IQuery<EntityHistory> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityHistoryQueryParams)Convert.ChangeType(defaultParams, typeof(EntityHistoryQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EntityHistory>> ToList()
        {

            var builder = new EntityHistoryQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var keywords = Params.Keywords.Value ?? string.Empty;
            
            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("Keywords", DbType.String, keywords)
            });

        }


    }

    #endregion

    #region "EntityHistoryQueryParams"

    public class EntityHistoryQueryParams
    {


        private WhereInt _id;
        private WhereInt _entityId;
        private WhereInt _entityReplyId;
        private WhereString _keywords;


        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt EntityId
        {
            get => _entityId ?? (_entityId = new WhereInt());
            set => _entityId = value;
        }

        public WhereInt EntityReplyId
        {
            get => _entityReplyId ?? (_entityReplyId = new WhereInt());
            set => _entityReplyId = value;
        }
        
        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }


    }

    #endregion

    #region "EntityHistoryQueryBuilder"

    public class EntityHistoryQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _entityHistoryTableName;
        private readonly string _usersTableName;

        private readonly EntityHistoryQuery _query;

        public EntityHistoryQueryBuilder(EntityHistoryQuery query)
        {
            _query = query;
            _entityHistoryTableName = GetTableNameWithPrefix("EntityHistory");
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
                    : "h.Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(h.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("h.*,")
                .Append("u.UserName,")
                .Append("u.DisplayName,")
                .Append("u.Alias,")
                .Append("u.PhotoUrl,")
                .Append("u.PhotoColor");
            return sb.ToString();

        }

        string BuildTables()
        {

            var sb = new StringBuilder();
            sb.Append(_entityHistoryTableName)
                .Append(" h WITH (nolock) LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" u ON h.CreatedUserId = u.Id");
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

            // Id
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("h.Id"));
            }
            
            // EntityId
            if (_query.Params.EntityId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("h.EntityId"));
            }
            
            // EntityReplyId
            if (_query.Params.EntityReplyId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityReplyId.Operator);
                sb.Append(_query.Params.EntityReplyId.ToSqlString("h.EntityReplyId"));
            }

            // Keywords
            if (!String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("[Message]", "Keywords"));
            }

            return sb.ToString();

        }
        
        private string BuildOrderBy()
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
                if (String.IsNullOrEmpty(columnName))
                {
                    throw new Exception($"No sort column could be found for the supplied key of '{sortColumn.Key}'");
                }
                ourput.Add(columnName, sortColumn.Value);

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
                    return "h.Id";
                case "message":
                    return "[Message]";
                case "EntityId":
                    return "h.EntityId";
                case "EntityReplyId":
                    return "h.EntityReplyId";
                case "majorversion":
                    return "h.MajorVersion";
                case "minorversion":
                    return "h.MinorVersion";
                case "createddate":
                    return "h.CreatedDate";
            
            }

            return string.Empty;

        }


        #endregion
    }

    #endregion

}
