using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Labels.Stores
{

    #region "LabelQuery"

    public class LabelQuery<TModel> : DefaultQuery<TModel> where TModel : class
    {

        private readonly IStore<TModel> _store;

        public LabelQuery(IStore<TModel> store)
        {
            _store = store;
        }

        public LabelQueryParams Params { get; set; }

        public override IQuery<TModel> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (LabelQueryParams)Convert.ChangeType(defaultParams, typeof(LabelQueryParams));
            return this;
        }

        public override async Task<IPagedResults<TModel>> ToList()
        {

            var builder = new LabelQueryBuilder<TModel>(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Name.Value,
                Params.Description.Value
            );

            return data;
        }
        
    }

    #endregion

    #region "LabelQueryParams"

    public class LabelQueryParams
    {
        
        private WhereInt _id;
        private WhereInt _featureId;
        private WhereString _name;
        private WhereString _description;

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

        public WhereString Name
        {
            get => _name ?? (_name = new WhereString());
            set => _name = value;
        }

        public WhereString Description
        {
            get => _description ?? (_description = new WhereString());
            set => _description = value;
        }

    }

    #endregion

    #region "LabelQueryBuilder"

    public class LabelQueryBuilder<TModel> : IQueryBuilder where TModel : class
    {
        #region "Constructor"

        private readonly string _labelsTableName;

        private readonly LabelQuery<TModel> _query;

        public LabelQueryBuilder(LabelQuery<TModel> query)
        {
            _query = query;
            _labelsTableName = GetTableNameWithPrefix("Labels");
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
            sb.Append("SELECT COUNT(l.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("l.*");
            return sb.ToString();

        }

        string BuildTables()
        {

            var sb = new StringBuilder();

            sb.Append(_labelsTableName)
                .Append(" l ");

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

        private string BuildWhereClause()
        {
            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("l.Id"));
            }

            if (_query.Params.FeatureId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.FeatureId.Operator);
                sb.Append(_query.Params.FeatureId.ToSqlString("l.FeatureId"));
            }
            
            if (!String.IsNullOrEmpty(_query.Params.Name.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Name.Operator);
                sb.Append(_query.Params.Name.ToSqlString("Name"));
            }

            if (!String.IsNullOrEmpty(_query.Params.Description.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Description.Operator);
                sb.Append(_query.Params.Description.ToSqlString("Description"));
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
                : "l." + columnName;
        }

        string BuildOrderBy()
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
                    return "l.Id";
                case "name":
                    return "l.[Name]";
                case "description":
                    return "l.[Description]";
                case "sortorder":
                    return "l.SortOrder";
                case "entities":
                    return "l.TotalEntities";
                case "totalentities":
                    return "l.TotalEntities";
                case "follows":
                    return "l.TotalFollows";
                case "totalfollows":
                    return "l.TotalFollows";
                case "views":
                    return "l.TotalViews";
                case "totalviews":
                    return "l.TotalViews";
                case "created":
                    return "l.CreatedDate";
                case "createddate":
                    return "l.CreatedDate";
                case "modified":
                    return "l.ModifiedDate";
                case "modifieddate":
                    return "l.ModifiedDate";
                case "lastentity":
                    return "l.LastEntityDate";
                case "lastentitydate":
                    return "l.LastEntityDate";
            }

            return string.Empty;

        }
        
        #endregion

    }

    #endregion

}
