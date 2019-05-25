using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Categories.Stores
{

    #region "CategoryQuery"

    public class CategoryQuery<TModel> : DefaultQuery<TModel> where TModel : class
    {

        private readonly IStore2<TModel> _store;

        public IQueryAdapterManager<TModel> QueryAdapterManager { get; set; }
        
        public CategoryQuery(IStore2<TModel> store)
        {
            _store = store;
        }

        public CategoryQueryParams Params { get; set; }

        public override IQuery<TModel> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (CategoryQueryParams)Convert.ChangeType(defaultParams, typeof(CategoryQueryParams));
            return this;
        }

        public override async Task<IPagedResults<TModel>> ToList()
        {

            var builder = new CategoryQueryBuilder<TModel>(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var keywords = Params.Keywords.Value ?? string.Empty;

            return await _store.SelectAsync(new[]
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

    #region "CategoryQueryParams"

    public class CategoryQueryParams
    {
        
        private WhereInt _id;
        private WhereInt _featureId;
        private WhereInt _userId;
        private WhereString _keywords;
        
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
        
        public WhereInt UserId
        {
            get => _userId ?? (_userId = new WhereInt());
            set => _userId = value;
        }

        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }
        
    }

    #endregion

    #region "CategoryQueryBuilder"

    public class CategoryQueryBuilder<TModel> : IQueryBuilder where TModel : class
    {
        #region "Constructor"

        private readonly string _categorysTableName;

        private readonly CategoryQuery<TModel> _query;

        public CategoryQueryBuilder(CategoryQuery<TModel> query)
        {
            _query = query;
            _categorysTableName = GetTableNameWithPrefix("Categories");

        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhere();
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
            var whereClause = BuildWhere();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(c.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {

            var sb = new StringBuilder();
            sb.Append("c.*");

            // -----------------
            // Apply any select query adapters
            // -----------------

            _query.QueryAdapterManager?.BuildSelect(_query, sb);
           
            return sb.ToString();
        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_categorysTableName)
                .Append(" c ");

            // -----------------
            // Apply any table query adapters
            // -----------------

            _query.QueryAdapterManager?.BuildTables(_query, sb);
         
            return sb.ToString();
        }

        string BuildWhere()
        {

            var sb = new StringBuilder();

            // -----------------
            // Apply any query adapters
            // -----------------

             _query.QueryAdapterManager?.BuildWhere(_query, sb);
          
            // -----------------
            // Id
            // -----------------

            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("c.Id"));
            }


            // -----------------
            // FeatureId
            // -----------------

            if (_query.Params.FeatureId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.FeatureId.Operator);
                sb.Append(_query.Params.FeatureId.ToSqlString("c.FeatureId"));
            }


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
        
   
        string GetQualifiedColumnName(string columnName)
        {

            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            return columnName.IndexOf('.') >= 0
                ? columnName
                : "c." + columnName;

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
