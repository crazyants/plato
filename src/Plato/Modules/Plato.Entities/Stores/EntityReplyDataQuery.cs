using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    #region "EntityReplyDataQuery"

    public class EntityReplyDataQuery : DefaultQuery<IEntityReplyData>
    {

        private readonly IStore<IEntityReplyData> _store;

        public EntityReplyDataQuery(IStore<IEntityReplyData> store)
        {
            _store = store;
        }

        public EntityReplyDataQueryParams Params { get; set; }

        public override IQuery<IEntityReplyData> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityReplyDataQueryParams)Convert.ChangeType(defaultParams, typeof(EntityReplyDataQueryParams));
            return this;
        }

        public override async Task<IPagedResults<IEntityReplyData>> ToList()
        {

            var builder = new EntityReplyDataQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var key = Params.Key.Value ?? string.Empty;

            return await _store.SelectAsync(new IDbDataParameter[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("Key", DbType.String, key)
            });

        }

    }

    #endregion

    #region "EntityReplyDataQueryParams"

    public class EntityReplyDataQueryParams
    {

        private WhereInt _id;
        private WhereInt _replyId;
        private WhereString _key;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt ReplyId
        {
            get => _replyId ?? (_replyId = new WhereInt());
            set => _replyId = value;
        }

        public WhereString Key
        {
            get => _key ?? (_key = new WhereString());
            set => _key = value;
        }

    }

    #endregion

    #region "EntityReplyDataQueryBuilder"

    public class EntityReplyDataQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _entityReplyDataTableName;

        private readonly EntityReplyDataQuery _query;

        public EntityReplyDataQueryBuilder(EntityReplyDataQuery query)
        {
            _query = query;
            _entityReplyDataTableName = GetTableNameWithPrefix("EntityReplyData");
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
            sb.Append("SELECT COUNT(d.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        string BuildPopulateSelect()
        {
            return "*";

        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_entityReplyDataTableName).Append(" d ");
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
                sb.Append(_query.Params.Id.ToSqlString("d.Id"));
            }

            // ReplyId
            if (_query.Params.ReplyId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ReplyId.Operator);
                sb.Append(_query.Params.ReplyId.ToSqlString("ReplyId"));
            }

            // Keywords
            if (!string.IsNullOrEmpty(_query.Params.Key.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Key.Operator);
                sb.Append(_query.Params.Key.ToSqlString("Key", "Key"));
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
