using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    #region "EntityQuery"

    public class EntityQuery<TModel> : DefaultQuery<TModel> where TModel : class
    {

        public EntityQueryParams Params { get; set; }

        private readonly IStore<TModel> _store;

        public EntityQuery(IStore<TModel> store)
        {
            _store = store;
        }

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
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            return await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Keywords.Value);
        }
        
    }

    #endregion

    #region "EntityQueryParams"

    public class EntityQueryParams
    {

        private WhereInt _id;
        private WhereInt _userId;
        private WhereInt _featureId;
        private WhereInt _categoryId;
        private WhereInt _roleId;
        private WhereInt _labelId;
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

        public WhereInt UserId
        {
            get => _userId ?? (_id = new WhereInt());
            set => _userId = value;
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

        public WhereInt RoleId
        {
            get => _roleId ?? (_roleId = new WhereInt());
            set => _roleId = value;
        }

        public WhereInt LabelId
        {
            get => _labelId ?? (_labelId = new WhereInt());
            set => _labelId = value;
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

        /*

            DECLARE @RowIndex int = 0;
            DECLARE @PageSize int = 40;

            DECLARE @FullTextSearchQuery nvarchar(4000);
            SET @FullTextSearchQuery = 'FORMSOF(INFLECTIONAL, installation)';

            DECLARE @FullTextMaxRank int;
            SET @FullTextMaxRank = (
            SELECT TOP 1 IsNull(ftEntities.RANK,0) AS MaxRank 
            FROM plato4_Entities e 
            LEFT OUTER JOIN plato4_Users c ON e.CreatedUserId = c.Id 
            LEFT OUTER JOIN plato4_Users m ON e.ModifiedUserId = m.Id 

            INNER JOIN CONTAINSTABLE(plato4_Entities, *, @FullTextSearchQuery) AS ftEntities ON ftEntities.[Key] = e.Id  

            WHERE (FeatureId = 41 AND IsPrivate = 0 AND IsSpam = 0) ORDER BY MaxRank DESC
            );

            SELECT e.*, 
            c.UserName AS CreatedUserName, 
            c.DisplayName AS CreatedDisplayName,
            c.FirstName AS CreatedFirstName,
            c.LastName AS CreatedLastName,
            c.Alias AS CreatedAlias,
            m.UserName AS ModifiedUserName, 
            m.DisplayName AS ModifiedDisplayName,
            m.FirstName AS ModifiedFirstName,
            m.LastName AS ModifiedLastName,
            m.Alias AS ModifiedAlias,
            IsNull(ftEntities.RANK,0) AS [Rank],
            @FullTextMaxRank AS MaxRank 

            FROM 

            plato4_Entities e
            LEFT OUTER JOIN plato4_Users c ON e.CreatedUserId = c.Id 
            LEFT OUTER JOIN plato4_Users m ON e.ModifiedUserId = m.Id 

            LEFT OUTER JOIN

            CONTAINSTABLE(plato4_Entities, *, @FullTextSearchQuery) AS ftEntities  ON ftEntities.[Key] = e.Id 

            LEFT OUTER JOIN

            CONTAINSTABLE(plato4_EntityReplies, *, @FullTextSearchQuery) AS ftEntityReplies 
            INNER JOIN plato4_EntityReplies er ON ftEntityReplies.[Key] = er.Id 

            ON e.Id = er.EntityId 

            WHERE (

	            (
		            e.FeatureId = 41 AND e.IsPrivate = 0 AND e.IsSpam = 0
	            )
	            AND
	            (
		            e.Id IN (IsNull(ftEntities.[Key],0)) OR 
		            er.Id IN (IsNull(ftEntityReplies.[Key],0))
	            )

            ) ORDER BY e.Id DESC OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;
            
        */

        #region "Constructor"

        private readonly string _entitiesTableName;
        private readonly string _usersTableName;
        private readonly string _userRolesTableName;
        private readonly string _rolesTableName;
        private readonly string _entityRepliesTableName;
        private readonly string _entityLabelsTableName;
        private readonly string _categoryRolesTableName;

        private readonly EntityQuery<TModel> _query;

        public EntityQueryBuilder(EntityQuery<TModel> query)
        {
            _query = query;
            _entitiesTableName = GetTableNameWithPrefix("Entities");
            _usersTableName = GetTableNameWithPrefix("Users");
            _rolesTableName = GetTableNameWithPrefix("Roles");
            _userRolesTableName = GetTableNameWithPrefix("UserRoles");
            _entityRepliesTableName = GetTableNameWithPrefix("EntityReplies");
            _entityLabelsTableName = GetTableNameWithPrefix("EntityLabels");
            _categoryRolesTableName = GetTableNameWithPrefix("CategoryRoles");

        }

        #endregion

        #region "Implementation"
        
        public string BuildSqlPopulate()
        {
            
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append(BuildFullTextQuery());
            sb.Append(BuildFullTextMaxRank());
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
            sb.Append(BuildFullTextQuery());
            sb.Append(BuildFullTextMaxRank());
            sb.Append("SELECT COUNT(e.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"
        
        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb
                .Append("e.*, ")
                .Append("c.UserName AS CreatedUserName, ")
                .Append("c.DisplayName AS CreatedDisplayName,")
                .Append("c.FirstName AS CreatedFirstName,")
                .Append("c.LastName AS CreatedLastName,")
                .Append("c.Alias AS CreatedAlias,")
                .Append("m.UserName AS ModifiedUserName, ")
                .Append("m.DisplayName AS ModifiedDisplayName,")
                .Append("m.FirstName AS ModifiedFirstName,")
                .Append("m.LastName AS ModifiedLastName,")
                .Append("m.Alias AS ModifiedAlias, ")
                .Append(BuildFullTextRankSelect()).Append(" AS [Rank], ")
                .Append("@FullTextMaxRank AS MaxRank");
            
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

            // join last modified user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" m ON e.ModifiedUserId = m.Id");

            if (EnableFullText())
            {

     
                // join ftEntities
                // ---------------------------

                sb
                    .Append(" LEFT OUTER JOIN ") // join entities
                    .Append(_query.Options.SearchType.ToString().ToUpper())
                    .Append("(")
                    .Append(_entitiesTableName)
                    .Append(", *, @FullTextSearchQuery");
                if (_query.Options.MaxResults > 0)
                {
                    sb.Append(", ").Append(_query.Options.MaxResults.ToString());
                }

                sb.Append(") AS ftEntities ON ftEntities.[Key] = e.Id");
                
                // join ftEntityReplies
                // ---------------------------

                sb
                    .Append(" LEFT OUTER JOIN ") // join entities
                    .Append(_query.Options.SearchType.ToString().ToUpper())
                    .Append("(")
                    .Append(_entityRepliesTableName)
                    .Append(", *, @FullTextSearchQuery");
                if (_query.Options.MaxResults > 0)
                {
                    sb.Append(", ").Append(_query.Options.MaxResults.ToString());
                }

                sb.Append(") AS ftEntityReplies ")
                    .Append("INNER JOIN ")
                    .Append(_entityRepliesTableName)
                    .Append(" AS er ON ftEntityReplies.[Key] = er.Id ");

                // join EntityReplies
                // ---------------------------

                sb.Append("ON er.EntityId = e.Id");

            }

            return sb.ToString();

        }

        string BuildFullTextMaxRank()
        {

            var sb = new StringBuilder();
            sb.Append("DECLARE @FullTextMaxRank int;");
            sb.Append("SET @FullTextMaxRank = ");

            if (EnableFullText())
            {
                var whereClause = BuildWhereClause();
                sb
                    .Append("(")
                    .Append("SELECT TOP 1 ")
                    .Append(BuildFullTextRankSelect())
                    .Append(" AS MaxRank FROM ")
                    .Append(BuildTables());
                if (!string.IsNullOrEmpty(whereClause))
                    sb.Append(" WHERE (").Append(whereClause).Append(")");
                sb.Append(" ORDER BY MaxRank DESC")
                    .Append(")");
            }
            else
            {
                sb.Append("0");
            }

            sb.Append(";");

            return sb.ToString();
        }

        string BuildFullTextQuery()
        {

            // Not used if full text search is not active
            if (!EnableFullText())
            {
                return string.Empty;
            }

            // Get full text query
            var fullTextSearchQuery =
                _query.Options.FullTextQueryParser.ToFullTextSearchQuery(
                    _query.Params.Keywords.Value);

            if (String.IsNullOrEmpty(fullTextSearchQuery))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.Append("DECLARE @FullTextSearchQuery nvarchar(4000);")
                .Append("SET @FullTextSearchQuery = '")
                .Append(fullTextSearchQuery.Replace("'", "''"))
                .Append("';")
                .Append(Environment.NewLine);
            return sb.ToString();

        }

        string BuildFullTextRankSelect()
        {

            if (!EnableFullText())
            {
                return "0";
            }

            return "(IsNull(ftEntities.RANK,0) + IsNull(ftEntityReplies.RANK,0))";
            
        }
        
        bool EnableFullText()
        {

            // No keywords
            if (String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                return false;
            }

            // Full text is not enabled
            if (_query.Options.SearchType == SearchTypes.Tsql)
            {
                return false;
            }

            return true;

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

            // Id
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("e.Id"));
            }

            // RoleId
            if (_query.Params.RoleId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.RoleId.Operator);
                sb.Append("(e.CategoryId IN (")
                    .Append("SELECT cr.CategoryId FROM ")
                    .Append(_categoryRolesTableName)
                    .Append(" AS cr WITH (nolock) WHERE ")
                    .Append(_query.Params.RoleId.ToSqlString("RoleId"))
                    .Append("))");
            }

            // FeatureId
            if (_query.Params.FeatureId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.FeatureId.Operator);
                sb.Append(_query.Params.FeatureId.ToSqlString("e.FeatureId"));
            }

            // CategoryId
            if (_query.Params.CategoryId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.CategoryId.Operator);
                sb.Append(_query.Params.CategoryId.ToSqlString("e.CategoryId"));
            }

            // LabelId
            // --> Only available if the Labels feature is enabled
            if (_query.Params.LabelId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.LabelId.Operator);
                sb.Append(" e.Id IN (")
                    .Append("SELECT EntityId FROM ")
                    .Append(_entityLabelsTableName)
                    .Append(" WHERE (")
                    .Append(_query.Params.LabelId.ToSqlString("e.LabelId"))
                    .Append("))");
            }

            // CreatedUserId
            if (_query.Params.CreatedUserId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.CreatedUserId.Operator);
                sb.Append(_query.Params.CreatedUserId.ToSqlString("e.CreatedUserId"));
            }

            // -----------------
            // private 
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
            // toggle spam 
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
            // pinned 
            // -----------------

            // hide = true, show = false
            if (_query.Params.IsNotPinned.Value && !_query.Params.IsPinned.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.IsNotPinned.Operator);
                sb.Append("e.IsPinned = 0");
            }

            // show = true, hide = false
            if (_query.Params.IsPinned.Value && !_query.Params.IsNotPinned.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.IsPinned.Operator);
                sb.Append("e.IsPinned = 1");
            }

            // -----------------
            // Keywords 
            // -----------------

            if (!string.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" AND ");

                sb.Append("(");

                if (_query.Options.SearchType == SearchTypes.Tsql)
                {

                    // Entities

                    sb.Append("(")
                        .Append(_query.Params.Keywords.ToSqlString("Title", "Keywords"))
                        .Append(" OR ")
                        .Append(_query.Params.Keywords.ToSqlString("Message", "Keywords"))
                        .Append(")");
                    
                    sb.Append(" OR ");

                    // Entity Replies

                    sb.Append("(e.Id IN (SELECT EntityId FROM ")
                        .Append(_entityRepliesTableName)
                        .Append(" WHERE (")
                        .Append(_query.Params.Keywords.ToSqlString("Message", "Keywords"))
                        .Append(")))");

                }
                else
                {
                    sb.Append("e.Id IN (IsNull(ftEntities.[Key],0))")
                        .Append(" OR ")
                        .Append("er.Id IN(IsNull(ftEntityReplies.[Key], 0))");

                }

                sb.Append(")");

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
            foreach (var sortColumn in GetSafeSortColumns())
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

            switch (columnName.ToLowerInvariant())
            {
                case "id":
                    return "e.Id";
                case "title":
                    return "e.Title";
                case "message":
                    return "e.[Message]";
                case "replies":
                    return "e.TotalReplies";
                case "totalreplies":
                    return "e.TotalReplies";
                case "participants":
                    return "e.TotalParticipants";
                case "totalparticipants":
                    return "e.TotalParticipants";
                case "views":
                    return "e.TotalViews";
                case "totalviews":
                    return "e.TotalViews";
                case "follows":
                    return "e.TotalFollows";
                case "totalfollows":
                    return "e.TotalFollows";
                case "reactions":
                    return "e.TotalReactions";
                case "totalreactions":
                    return "e.TotalReactions";
                case "created":
                    return "e.CreatedDate";
                case "createddate":
                    return "e.CreatedDate";
                case "modified":
                    return "e.ModifiedDate";
                case "modifieddate":
                    return "e.ModifiedDate";
                case "lastreply":
                    return "e.LastReplyDate";
                case "lastreplydate":
                    return "e.LastReplyDate";
                case "rank":
                    return "[Rank]";
            }

            return string.Empty;

        }
        
        #endregion

    }

    #endregion

}
