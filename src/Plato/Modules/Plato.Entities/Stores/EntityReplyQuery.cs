using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    #region "EntityReplyQuery"

    public class EntityReplyQuery : DefaultQuery
    {

        private readonly IStore<EntityReply> _store;

        public EntityReplyQuery(IStore<EntityReply> store)
        {
            _store = store;
        }

        public EntityReplyQueryParams Params { get; set; }

        public override IQuery Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityReplyQueryParams)Convert.ChangeType(defaultParams, typeof(EntityReplyQueryParams));
            return this;
        }

        public override async Task<IPagedResults<T>> ToList<T>()
        {

            var builder = new EntityReplyQueryBuilder(this);
            var startSql = builder.BuildSqlStartId();
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync<T>(
                PageIndex,
                PageSize,
                startSql,
                populateSql,
                countSql,
                Params.Keywords.Value
            );

            return data;
        }
    }

    #endregion

    #region "EntityReplyQueryParams"

    public class EntityReplyQueryParams
    {


        private WhereInt _id;
        private WhereInt _entityId;
        private WhereString _keywords;
        private WhereBool _isPrivate;
        private WhereBool _isSpam;
        private WhereBool _isPinned;
        private WhereBool _isDeleted;
        private WhereBool _isClosed;
        private WhereInt _createdUserId;
        private WhereDate _createdDate;
        private WhereInt _modifiedUserId;
        private WhereDate _modifiedDate;
        
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
        
        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }

        public WhereBool IsPrivate
        {
            get => _isPrivate ?? (_isPrivate = new WhereBool());
            set => _isPrivate = value;
        }

        public WhereBool IsSpam
        {
            get => _isSpam ?? (_isSpam = new WhereBool());
            set => _isSpam = value;
        }

        public WhereBool IsPinned
        {
            get => _isPinned ?? (_isPinned = new WhereBool());
            set => _isPinned = value;
        }

        public WhereBool IsDeleted
        {
            get => _isDeleted ?? (_isDeleted = new WhereBool());
            set => _isDeleted = value;
        }

        public WhereBool IsClosed
        {
            get => _isClosed ?? (_isClosed = new WhereBool());
            set => _isClosed = value;
        }

        public WhereInt CreatedUserId
        {
            get => _createdUserId ?? (_createdUserId = new WhereInt());
            set => _createdUserId = value;
        }

        public WhereDate CreatedDate
        {
            get => _createdDate ?? (_createdDate = new WhereDate());
            set => _createdDate = value;
        }

        public WhereInt ModifiedUserId
        {
            get => _modifiedUserId ?? (_modifiedUserId = new WhereInt());
            set => _modifiedUserId = value;
        }

        public WhereDate ModifiedDate
        {
            get => _modifiedDate ?? (_modifiedDate = new WhereDate());
            set => _modifiedDate = value;
        }

    }

    #endregion

    #region "EntityReplyQueryBuilder"

    public class EntityReplyQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _tableName;
        private const string TableName = "EntityReplies";

        private readonly EntityReplyQuery _query;

        public EntityReplyQueryBuilder(EntityReplyQuery query)
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

            var tablePrefix = _query.TablePrefix;

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

            if (_query.Params.EntityId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("EntityId"));
            }

            if (!string.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("Keywords"));
            }

           
            if (!string.IsNullOrEmpty(sb.ToString()))
                sb.Append(_query.Params.IsPrivate.Operator);
            sb.Append(_query.Params.IsPrivate.ToSqlString("IsPrivate"));
      
         
   
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
