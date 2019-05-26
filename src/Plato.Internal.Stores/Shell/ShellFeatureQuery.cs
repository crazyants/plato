using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Shell
{

    #region "ShellFeatureQuery"

    public class ShellFeatureQuery : DefaultQuery<ShellFeature>
    {

        private readonly IStore<ShellFeature> _store;

        public ShellFeatureQuery(IStore<ShellFeature> store)
        {
            _store = store;
        }

        public ShellFeatureQueryParams Params { get; set; }

        public override IQuery<ShellFeature> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (ShellFeatureQueryParams)Convert.ChangeType(defaultParams, typeof(ShellFeatureQueryParams));
            return this;
        }

        public override async Task<IPagedResults<ShellFeature>> ToList()
        {

            var builder = new ShellFeatureQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var id = Params?.Id.Value;
            var moduleId = Params?.ModuleId?.Value ?? string.Empty;

            return await _store.SelectAsync(new IDbDataParameter[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("Id", DbType.Int32, id),
                new DbParam("ModuleId", DbType.String, 255, moduleId)
            });
        }


    }

    #endregion

    #region "ShellFeatureQueryParams"

    public class ShellFeatureQueryParams
    {
        
        private WhereInt _id;
        private WhereString _moduleId;
        
        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereString ModuleId
        {
            get => _moduleId ?? (_moduleId = new WhereString());
            set => _moduleId = value;
        }

    }

    #endregion

    #region "ShellFeatureQueryBuilder"

    public class ShellFeatureQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _shellFeaturesTableName;
  
        private readonly ShellFeatureQuery _query;

        public ShellFeatureQueryBuilder(ShellFeatureQuery query)
        {
            _query = query;
            _shellFeaturesTableName = GetTableNameWithPrefix("ShellFeatures");

        }

        #endregion

        #region "Implementation"
        
        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM ").Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            sb.Append(" ORDER BY ").Append(
                    !string.IsNullOrEmpty(orderBy)
                        ? orderBy
                        : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(f.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_shellFeaturesTableName)
                .Append(" f ");
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

            if (_query.Params == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("f.Id"));
            }

            // ModuleId
            if (!string.IsNullOrEmpty(_query.Params.ModuleId.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.ModuleId.ToSqlString("f.ModuleId", "ModuleId"));
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
                : "f." + columnName;
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
