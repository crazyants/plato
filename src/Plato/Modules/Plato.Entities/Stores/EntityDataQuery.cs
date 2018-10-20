using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    #region "EntityDataQuery"

    public class EntityDataQuery : DefaultQuery<IEntityData>
    {

        private readonly IStore<IEntityData> _store;

        public EntityDataQuery(IStore<IEntityData> store)
        {
            _store = store;
        }

        public EntityDataQueryParams Params { get; set; }

        public override IQuery<IEntityData> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityDataQueryParams)Convert.ChangeType(defaultParams, typeof(EntityDataQueryParams));
            return this;
        }

        public override async Task<IPagedResults<IEntityData>> ToList()
        {

            var builder = new EntityDataQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Key.Value
            );

            return data;
        }
        
    }

    #endregion

    #region "EntityDataQueryParams"

    public class EntityDataQueryParams
    {

        private WhereInt _id;
        private WhereInt _entityId;
        private WhereString _key;
     
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

        public WhereString Key
        {
            get => _key ?? (_key = new WhereString());
            set => _key = value;
        }

    }

    #endregion

    #region "EntityDataQueryBuilder"

    public class EntityDataQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _entityDataTableName;
   
        private readonly EntityDataQuery _query;

        public EntityDataQueryBuilder(EntityDataQuery query)
        {
            _query = query;
            _entityDataTableName = GetTableNameWithPrefix("EntityData");
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
            sb.Append(_entityDataTableName).Append(" d ");
            return sb.ToString();
        }

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
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("d.Id"));
            }

            // EntityId
            if (_query.Params.EntityId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("EntityId"));
            }

            // Keywords
            if (!string.IsNullOrEmpty(_query.Params.Key.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Key.Operator);
                sb.Append(_query.Params.Key.ToSqlString("Key"));
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
