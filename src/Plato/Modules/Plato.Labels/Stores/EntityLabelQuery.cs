using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Labels.Models;

namespace Plato.Labels.Stores
{

    #region "EntityLabelQuery"

    public class EntityLabelQuery : DefaultQuery<EntityLabel>
    {

        private readonly IStore<EntityLabel> _store;

        public EntityLabelQuery(IStore<EntityLabel> store)
        {
            _store = store;
        }

        public EntityLabelQueryParams Params { get; set; }

        public override IQuery<EntityLabel> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityLabelQueryParams)Convert.ChangeType(defaultParams, typeof(EntityLabelQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EntityLabel>> ToList()
        {

            var builder = new EntityLabelQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var labelId = Params.LabelId.Value;
            var entityId = Params.EntityId.Value;

            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("LabelId", DbType.Int32, labelId),
                new DbParam("EntityId", DbType.Int32, entityId)
            });

        }
        
    }

    #endregion

    #region "EntityLabelQueryParams"

    public class EntityLabelQueryParams
    {

        private WhereInt _id;
        private WhereInt _labelId;
        private WhereInt _entityId;

        private WhereBool _showHidden;
        private WhereBool _hideHidden;
        private WhereBool _showPrivate;
        private WhereBool _hidePrivate;
        private WhereBool _showSpam;
        private WhereBool _hideSpam;
        private WhereBool _showClosed;
        private WhereBool _hideClosed;
        private WhereBool _hideDeleted;
        private WhereBool _showDeleted;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt LabelId
        {
            get => _labelId ?? (_labelId = new WhereInt());
            set => _labelId = value;
        }

        public WhereInt EntityId
        {
            get => _entityId ?? (_entityId = new WhereInt());
            set => _entityId = value;
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

        public WhereBool HideClosed
        {
            get => _hideClosed ?? (_hideClosed = new WhereBool());
            set => _hideClosed = value;
        }

        public WhereBool ShowClosed
        {
            get => _showClosed ?? (_showClosed = new WhereBool());
            set => _showClosed = value;
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

    #region "EntityLabelQueryBuilder"

    public class EntityLabelQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _entitiesTableName;
        private readonly string _entityLabelsTableName;

        private readonly EntityLabelQuery _query;

        public EntityLabelQueryBuilder(EntityLabelQuery query)
        {
            _query = query;
            _entitiesTableName = GetTableNameWithPrefix("Entities");
            _entityLabelsTableName = GetTableNameWithPrefix("EntityLabels");
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
            sb.Append("SELECT COUNT(el.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("el.*");
            return sb.ToString();

        }

        string BuildTables()
        {

            var sb = new StringBuilder();

            sb.Append(_entityLabelsTableName)
                .Append(" el ")
                .Append(" INNER JOIN ")
                .Append(_entitiesTableName)
                .Append(" e ON el.EntityId = e.Id ");

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
            
            // LabelId
            if (_query.Params.LabelId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.LabelId.Operator);
                sb.Append(_query.Params.LabelId.ToSqlString("el.LabelId"));
            }

            // EntityId
            if (_query.Params.EntityId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("el.EntityId"));
            }


            // -----------------
            // IsHidden 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideHidden.Value && !_query.Params.ShowHidden.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideHidden.Operator);
                sb.Append("e.IsHidden = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowHidden.Value && !_query.Params.HideHidden.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowHidden.Operator);
                sb.Append("e.IsHidden = 1");
            }

            // -----------------
            // IsPrivate 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HidePrivate.Value && !_query.Params.ShowPrivate.Value)
            {
                // Hide all private entities except those created by the current user
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
            // IsSpam 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideSpam.Value && !_query.Params.ShowSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideSpam.Operator);
                sb.Append("e.IsSpam = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowSpam.Value && !_query.Params.HideSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowSpam.Operator);
                sb.Append("e.IsSpam = 1");
            }

            // -----------------
            // IsDeleted 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideDeleted.Value && !_query.Params.ShowDeleted.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideDeleted.Operator);
                sb.Append("e.IsDeleted = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowDeleted.Value && !_query.Params.HideDeleted.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowDeleted.Operator);
                sb.Append("e.IsDeleted = 1");
            }

            // -----------------
            // IsClosed 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideClosed.Value && !_query.Params.ShowClosed.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideClosed.Operator);
                sb.Append("e.IsClosed = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowClosed.Value && !_query.Params.HideClosed.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowClosed.Operator);
                sb.Append("e.IsClosed = 1");
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
                : "el." + columnName;
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
