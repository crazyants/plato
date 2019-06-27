using System;
using System.Collections.Generic;
using System.Data;
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
            var keywords = Params.Keywords.Value ?? string.Empty;

            return await _store.SelectAsync(new IDbDataParameter[]
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

    #region "EntityTagQueryParams"

    public class EntityTagQueryParams
    {


        private WhereInt _id;
        private WhereInt _tagId;
        private WhereInt _entityId;
        private WhereInt _entityReplyId;
        private WhereString _keywords;
        private WhereBool _showHidden;
        private WhereBool _hideHidden;
        private WhereBool _showPrivate;
        private WhereBool _hidePrivate;
        private WhereBool _showSpam;
        private WhereBool _hideSpam;
        private WhereBool _hideDeleted;
        private WhereBool _showDeleted;

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

        public WhereBool ShowPrivate
        {
            get => _showPrivate ?? (_showPrivate = new WhereBool());
            set => _showPrivate = value;
        }

        public WhereBool HidePrivate
        {
            get => _hidePrivate ?? (_hidePrivate = new WhereBool());
            set => _hidePrivate = value;
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

    #region "EntityTagQueryBuilder"

    public class EntityTagQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _tagsTableName;
        private readonly string _entitiesTableName;
        private readonly string _entityRepliesTableName;
        private readonly string _entityTagsTableName;

        private readonly EntityTagQuery _query;

        public EntityTagQueryBuilder(EntityTagQuery query)
        {
            _query = query;
            _tagsTableName = GetTableNameWithPrefix("Tags");
            _entitiesTableName = GetTableNameWithPrefix("Entities");
            _entityRepliesTableName = GetTableNameWithPrefix("EntityReplies");
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
                .Append(" t ON t.Id = et.TagId ")
                .Append("INNER JOIN ")
                .Append(_entitiesTableName)
                .Append(" e ON e.Id = et.EntityId ")
                .Append("LEFT OUTER JOIN ")
                .Append(_entityRepliesTableName)
                .Append(" er ON er.Id = et.EntityReplyId ");

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

            // -----------------
            // Keywords
            // -----------------

            if (!String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("t.[Name]", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("t.Description", "Keywords"));
            }
            
            // -----------------
            // IsPrivate 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HidePrivate.Value && !_query.Params.ShowPrivate.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HidePrivate.Operator);
                sb.Append("e.IsPrivate = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowPrivate.Value && !_query.Params.HidePrivate.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowPrivate.Operator);
                sb.Append("e.IsPrivate = 1");
            }

            // -----------------
            // IsHidden 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideHidden.Value && !_query.Params.ShowHidden.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideHidden.Operator);
                sb.Append("(e.IsHidden = 0 AND IsNull(er.IsHidden, 0) = 0)");
            }

            // show = true, hide = false
            if (_query.Params.ShowHidden.Value && !_query.Params.HideHidden.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowHidden.Operator);
                sb.Append("(e.IsHidden = 1 AND IsNull(er.IsHidden, 1) = 1)");
            }
            
            // -----------------
            // IsSpam 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideSpam.Value && !_query.Params.ShowSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideSpam.Operator);
                sb.Append("(e.IsSpam = 0 AND IsNull(er.IsSpam, 0) = 0)");
            }

            // show = true, hide = false
            if (_query.Params.ShowSpam.Value && !_query.Params.HideSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowSpam.Operator);
                sb.Append("(e.IsSpam = 1 AND IsNull(er.IsSpam, 1) = 1)");
            }

            // -----------------
            // IsDeleted 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideDeleted.Value && !_query.Params.ShowDeleted.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideDeleted.Operator);
                sb.Append("(e.IsDeleted = 0 AND IsNull(er.IsDeleted, 0) = 0)");
            }

            // show = true, hide = false
            if (_query.Params.ShowDeleted.Value && !_query.Params.HideDeleted.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowDeleted.Operator);
                sb.Append("(e.IsDeleted = 1 AND IsNull(er.IsDeleted, 1) = 1)");
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
