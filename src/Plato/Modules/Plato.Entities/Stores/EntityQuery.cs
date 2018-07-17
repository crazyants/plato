using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    #region "EntityQuery"

    public class EntityQuery<TModel> : DefaultQuery<TModel> where TModel : class
    {

        private readonly IStore<TModel> _store;

        public EntityQuery(IStore<TModel> store)
        {
            _store = store;
        }
        
        public EntityQueryParams Params { get; set; }

        public override IQuery<TModel> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityQueryParams)Convert.ChangeType(defaultParams, typeof(EntityQueryParams));
            return this;
        }
        
        public override async Task<IPagedResults<TModel>> ToList()
        {

            var builder = new EntityQueryBuilder<TModel>(this);
            var startSql = builder.BuildSqlStartId();
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
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

    #region "EntityQueryParams"

    public class EntityQueryParams
    {


        private WhereInt _id;
        private WhereInt _featureId;
        private WhereInt _categoryId;
        private WhereString _keywords;
        private WhereBool _showPrivate;
        private WhereBool _hidePrivate;
        private WhereBool _showSpam;
        private WhereBool _hideSpam;
        private WhereBool _isPinned;
        private WhereBool _isNotPinned;
        private WhereBool _hideDeleted;
        private WhereBool _showDeleted;
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

        public WhereInt FeatureId
        {
            get => _featureId ?? (_featureId = new WhereInt());
            set => _featureId = value;
        }

        public WhereInt CategoryId
        {
            get => _categoryId ?? (_categoryId = new WhereInt());
            set => _categoryId = value;
        }


        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
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

        public WhereBool ShowSpam
        {
            get => _showSpam ?? (_showSpam = new WhereBool());
            set => _showSpam = value;
        }

        public WhereBool HideSpam
        {
            get => _hideSpam ?? (_hideSpam = new WhereBool());
            set => _hideSpam = value;
        }

        public WhereBool IsPinned
        {
            get => _isPinned ?? (_isPinned = new WhereBool());
            set => _isPinned = value;
        }

        public WhereBool IsNotPinned
        {
            get => _isNotPinned ?? (_isNotPinned = new WhereBool());
            set => _isNotPinned = value;
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

        public WhereBool IsClosed
        {
            get => _isClosed ?? (_isClosed = new WhereBool(false));
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

    #region "EntityQueryBuilder"

    public class EntityQueryBuilder<TModel> : IQueryBuilder where TModel : class
    {
        #region "Constructor"

        private readonly string _entitiesTableName;
        private readonly string _usersTableName;

        private readonly EntityQuery<TModel> _query;

        public EntityQueryBuilder(EntityQuery<TModel> query)
        {
            _query = query;
            _entitiesTableName = GetTableNameWithPrefix("Entities");
            _usersTableName = GetTableNameWithPrefix("Users");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlStartId()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT @start_id_out = e.Id FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            if (!string.IsNullOrEmpty(orderBy))
                sb.Append(" ORDER BY ").Append(orderBy);
            return sb.ToString();
        }

        public string BuildSqlPopulate()
        {

            var whereClause = BuildWhereClauseForStartId();
            var orderBy = BuildOrderBy();

            var sb = new StringBuilder();
            sb.Append("SELECT ")
                .Append(BuildPopulateSelect())
                .Append(" FROM ")
                .Append(BuildTables());

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
            sb.Append("SELECT COUNT(e.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }


        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("e.*, ")
                .Append("c.UserName AS CreatedUserName, ")
                .Append("c.NormalizedUserName AS CreatedNormalizedUserName,")
                .Append("c.DisplayName AS CreatedDisplayName,")
                .Append("m.UserName AS ModifiedUserName, ")
                .Append("m.NormalizedUserName AS ModifiedNormalizedUserName,")
                .Append("m.DisplayName AS ModifiedDisplayName");
            return sb.ToString();

        }

        string BuildTables()
        {

            var sb = new StringBuilder();

            sb.Append(_entitiesTableName)
                .Append(" e ");

            // join created user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" c ON e.CreatedUserId = c.Id ");
            
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" m ON e.ModifiedUserId = m.Id");
            
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

        private string BuildWhereClauseForStartId()
        {
            var sb = new StringBuilder();
            // default to ascending
            if (_query.SortColumns.Count == 0)
                sb.Append("e.Id >= @start_id_in");
            // set start operator based on first order by
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(sortColumn.Value != OrderBy.Asc
                    ? "e.Id <= @start_id_in"
                    : "e.Id >= @start_id_in");
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

            // Id
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("e.Id"));
            }

            // FeatureId
            if (_query.Params.FeatureId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.FeatureId.Operator);
                sb.Append(_query.Params.FeatureId.ToSqlString("FeatureId"));
            }

            // CategoryId
            if (_query.Params.CategoryId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.CategoryId.Operator);
                sb.Append(_query.Params.CategoryId.ToSqlString("CategoryId"));
            }

            // Keywords
            if (!string.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("Keywords"));
            }
            
            // -----------------
            // private 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HidePrivate.Value && !_query.Params.ShowPrivate.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HidePrivate.Operator);
                sb.Append("IsPrivate = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowPrivate.Value && !_query.Params.HidePrivate.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowPrivate.Operator);
                sb.Append("IsPrivate = 1");
            }

            // -----------------
            // toggle spam 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideSpam.Value && !_query.Params.ShowSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideSpam.Operator);
                sb.Append("IsSpam = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowSpam.Value && !_query.Params.HideSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowSpam.Operator);
                sb.Append("IsSpam = 1");
            }

            // -----------------
            // pinned 
            // -----------------

            // hide = true, show = false
            if (_query.Params.IsNotPinned.Value && !_query.Params.IsPinned.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.IsNotPinned.Operator);
                sb.Append("IsPinned = 0");
            }

            // show = true, hide = false
            if (_query.Params.IsPinned.Value && !_query.Params.IsNotPinned.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.IsPinned.Operator);
                sb.Append("IsPinned = 1");
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
