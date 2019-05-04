using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Tags.Models;

namespace Plato.Tags.Stores
{

    #region "EntityTagQuery"

    public class EntityTagQuery : DefaultQuery<EntityTag>
    {

        private readonly IStore<EntityTag> _store;

        public EntityTagQuery(IStore<EntityTag> store)
        {
            _store = store;
        }

        public EntityTagQueryParams Params { get; set; }

        public override IQuery<EntityTag> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityTagQueryParams)Convert.ChangeType(defaultParams, typeof(EntityTagQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EntityTag>> ToList()
        {

            var builder = new EntityTagQueryBuilder(this);
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

    #region "EntityTagQueryParams"

    public class EntityTagQueryParams
    {


        private WhereInt _id;
        private WhereInt _tagId;
        private WhereInt _entityId;
        private WhereInt _entityReplyId;
        private WhereString _keywords;


        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt TagId
        {
            get => _tagId ?? (_tagId = new WhereInt());
            set => _tagId = value;
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

    #region "EntityTagQueryBuilder"

    public class EntityTagQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _tagsTableName;
        private readonly string _entityTagsTableName;

        private readonly EntityTagQuery _query;

        public EntityTagQueryBuilder(EntityTagQuery query)
        {
            _query = query;
            _tagsTableName = GetTableNameWithPrefix("Tags");
            _entityTagsTableName = GetTableNameWithPrefix("EntityTags");
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
            sb.Append("SELECT COUNT(et.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("et.*, ")
                .Append("t.FeatureId,")
                .Append("t.[Name],")
                .Append("t.NameNormalized,")
                .Append("t.[Description],")
                .Append("t.Alias,")
                .Append("t.TotalEntities,")
                .Append("t.TotalFollows,")
                .Append("t.LastSeenDate,")
                .Append("t.CreatedUserId,")
                .Append("t.CreatedDate,")
                .Append("t.ModifiedUserId,")
                .Append("t.ModifiedDate");
            return sb.ToString();
        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_entityTagsTableName)
                .Append(" et WITH (nolock) ")
                .Append("INNER JOIN ")
                .Append(_tagsTableName)
                .Append(" t ON t.Id = et.TagId");
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
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("el.Id"));
            }

            // TagId
            if (_query.Params.TagId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.TagId.Operator);
                sb.Append(_query.Params.TagId.ToSqlString("et.TagId"));
            }


            // TagId
            if (_query.Params.TagId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.TagId.Operator);
                sb.Append(_query.Params.TagId.ToSqlString("et.TagId"));
            }

            // EntityId
            if (_query.Params.EntityId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("et.EntityId"));
            }

            // EntityReplyId
            if (_query.Params.EntityReplyId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityReplyId.Operator);
                sb.Append(_query.Params.EntityReplyId.ToSqlString("et.EntityReplyId"));
            }

            // Keywords
            if (!String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("t.[Name]", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("t.Description", "Keywords"));
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
            var output = new Dictionary<string, OrderBy>();
            foreach (var sortColumn in _query.SortColumns)
            {
                var columnName = GetSortColumn(sortColumn.Key);
                if (String.IsNullOrEmpty(columnName))
                {
                    throw new Exception($"No sort column could be found for the supplied key of '{sortColumn.Key}'");
                }
                output.Add(columnName, sortColumn.Value);

            }

            return output;
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
                case "name":
                    return "[Name]";
                case "description":
                    return "Description";
            }

            return string.Empty;

        }
        
        #endregion

    }

    #endregion

}
