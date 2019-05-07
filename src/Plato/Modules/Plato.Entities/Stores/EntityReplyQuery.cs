using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    #region "EntityReplyQuery"

    public class EntityReplyQuery<TModel> : DefaultQuery<TModel> where TModel : class
    {

        private readonly IStore<TModel> _store;

        public EntityReplyQuery(IStore<TModel> store)
        {
            _store = store;
        }

        public EntityReplyQueryParams Params { get; set; }

        public override IQuery<TModel> Select<TParams>(Action<TParams> configure)
        {
            var defaultParams = new TParams();
            configure(defaultParams);
            Params = (EntityReplyQueryParams)Convert.ChangeType(defaultParams, typeof(EntityReplyQueryParams));
            return this;
        }
        
        public override async Task<IPagedResults<TModel>> ToList()
        {
            var builder = new EntityReplyQueryBuilder<TModel>(this);
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

    #region "EntityReplyQueryParams"

    public class EntityReplyQueryParams
    {


        private WhereInt _id;
        private WhereInt _entityId;
        private WhereInt _categoryId;
        private WhereString _keywords;
        private WhereBool _hideHidden;
        private WhereBool _showHidden;
        private WhereBool _hideSpam;
        private WhereBool _showSpam;
        private WhereBool _hideDeleted;
        private WhereBool _showDeleted;
        private WhereBool _hideAnswers;
        private WhereBool _showAnswers;
        
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

        public WhereBool HideHidden
        {
            get => _hideHidden ?? (_hideHidden = new WhereBool());
            set => _hideHidden = value;
        }

        public WhereBool ShowHidden
        {
            get => _showHidden ?? (_showHidden = new WhereBool());
            set => _showHidden = value;
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

        public WhereBool HideAnswers
        {
            get => _hideAnswers ?? (_hideAnswers = new WhereBool());
            set => _hideAnswers = value;
        }

        public WhereBool ShowAnswers
        {
            get => _showAnswers ?? (_showAnswers = new WhereBool());
            set => _showAnswers = value;
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

    public class EntityReplyQueryBuilder<TModel> : IQueryBuilder where TModel : class
    {
        #region "Constructor"

        private readonly string _entitiesTableName;
        private readonly string _entityRepliesTableName;
        private readonly string _usersTableName;

        private readonly EntityReplyQuery<TModel> _query;

        public EntityReplyQueryBuilder(EntityReplyQuery<TModel> query)
        {
            _query = query;
            _entitiesTableName = GetTableNameWithPrefix("Entities");
            _entityRepliesTableName = GetTableNameWithPrefix("EntityReplies");
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
                    : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(r.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }
        
        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("r.*,")
                .Append("c.UserName AS CreatedUserName,")
                .Append("c.NormalizedUserName AS CreatedNormalizedUserName,")
                .Append("c.DisplayName AS CreatedDisplayName,")
                .Append("c.Alias AS CreatedAlias,")
                .Append("c.PhotoUrl AS CreatedPhotoUrl,")
                .Append("c.PhotoColor AS CreatedPhotoColor,")
                .Append("c.SignatureHtml AS CreatedSignatureHtml,")
                .Append("m.UserName AS ModifiedUserName,")
                .Append("m.NormalizedUserName AS ModifiedNormalizedUserName,")
                .Append("m.DisplayName AS ModifiedDisplayName,")
                .Append("m.Alias AS ModifiedAlias,")
                .Append("m.PhotoUrl AS ModifiedPhotoUrl,")
                .Append("m.PhotoColor AS ModifiedPhotoColor, ")
                .Append("m.SignatureHtml AS ModifiedSignatureHtml");
            return sb.ToString();

        }

        string BuildTables()
        {
            
            var sb = new StringBuilder();
            sb.Append(_entityRepliesTableName)
                .Append(" r ");

            // join created user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" c ON r.CreatedUserId = c.Id ");
            
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" m ON r.ModifiedUserId = m.Id");
            
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
            
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("r.Id"));
            }

            if (_query.Params.EntityId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("EntityId"));
            }

            if (!string.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("Message", "Keywords"));
            }
            
            // -----------------
            // CategoryId 
            // -----------------

            if (_query.Params.CategoryId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.CategoryId.Operator);
                sb.Append(" r.EntityId IN (")
                    .Append("SELECT Id FROM ")
                    .Append(_entitiesTableName)
                    .Append(" WHERE (")
                    .Append(_query.Params.CategoryId.ToSqlString("CategoryId"))
                    .Append("))");
            }

            // -----------------
            // IsHidden 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideHidden.Value && !_query.Params.ShowHidden.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideHidden.Operator);
                sb.Append("r.IsHidden = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowHidden.Value && !_query.Params.HideHidden.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowHidden.Operator);
                sb.Append("r.IsHidden = 1");
            }

            // -----------------
            // IsSpam 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideSpam.Value && !_query.Params.ShowSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideSpam.Operator);
                sb.Append("r.IsSpam = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowSpam.Value && !_query.Params.HideSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowSpam.Operator);
                sb.Append("r.IsSpam = 1");
            }

            // -----------------
            // IsDeleted 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideDeleted.Value && !_query.Params.ShowDeleted.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideDeleted.Operator);
                sb.Append("r.IsDeleted = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowDeleted.Value && !_query.Params.HideDeleted.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowDeleted.Operator);
                sb.Append("r.IsDeleted = 1");
            }

            // -----------------
            // IsAnswer 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideAnswers.Value && !_query.Params.ShowAnswers.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideAnswers.Operator);
                sb.Append("r.IsAnswer = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowAnswers.Value && !_query.Params.HideAnswers.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowAnswers.Operator);
                sb.Append("r.IsAnswer = 1");
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
                : "r." + columnName;
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

            switch (columnName.ToLowerInvariant())
            {
                case "id":
                    return "r.Id";
                case "message":
                    return "r.Message";
                case "html":
                    return "r.Html";
                case "abstract":
                    return "r.Abtract";
                case "hidden":
                    return "r.IsHidden";
                case "ishidden":
                    return "r.IsHidden";
                case "spam":
                    return "r.IsSpam";
                case "isspam":
                    return "r.IsSpam";
                case "answer":
                    return "r.IsAnswer";
                case "isanswer":
                    return "r.IsAnswer";
                case "pinned":
                    return "r.IsPinned";
                case "ispinned":
                    return "r.IsPinned";
                case "closed":
                    return "r.IsClosed";
                case "isclosed":
                    return "r.IsClosed";
                case "totalreactions":
                    return "r.TotalReactions";
                case "totalreports":
                    return "r.TotalReports";
                case "totalratings":
                    return "r.TotalRatings";
                case "summedrating":
                    return "r.SummedRating";
                case "meanrating":
                    return "r.MeanRating";
                case "totallinks":
                    return "r.TotalLinks";
                case "totalimages":
                    return "r.TotalImages";
                case "created":
                    return "r.CreatedDate";
                case "createddate":
                    return "r.CreatedDate";
                case "edited":
                    return "r.EditedDate";
                case "editeddate":
                    return "r.EditedDate";
                case "modified":
                    return "r.ModifiedDate";
                case "modifieddate":
                    return "r.ModifiedDate";
               
            }

            return string.Empty;

        }
        
        #endregion

    }

    #endregion

}
