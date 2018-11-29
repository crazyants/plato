using System;
using System.Collections.Generic;
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
            var startSql = builder.BuildSqlStartId();
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var keywords = Params?.Keywords?.Value ?? string.Empty;

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                startSql,
                populateSql,
                countSql,
                keywords
            );

            return data;
        }


    }

    #endregion

    #region "ShellFeatureQueryParams"

    public class ShellFeatureQueryParams
    {


        private WhereInt _id;
        private WhereString _keywords;


        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
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

        public string BuildSqlStartId()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT @start_id_out = f.Id FROM ")
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
            sb.Append("SELECT COUNT(f.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("f.* ");
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

        private string BuildWhereClauseForStartId()
        {
            var sb = new StringBuilder();
            // default to ascending
            if (_query.SortColumns.Count == 0)
                sb.Append("f.Id >= @start_id_in");
            // set start operator based on first order by
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(sortColumn.Value != OrderBy.Asc
                    ? "f.Id <= @start_id_in"
                    : "f.Id >= @start_id_in");
                break;
            }

            var where = BuildWhereClause();
            if (!string.IsNullOrEmpty(where))
                sb.Append(" AND ").Append(where);

            return sb.ToString();

        }

        private string BuildWhereClause()
        {

            if (_query.Params == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("f.Id"));
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
