using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Stores
{

    #region "ReactionQuery"

    public class ReactionQuery : DefaultQuery<Reaction>
    {

        private readonly IStore<Reaction> _store;

        public ReactionQuery(IStore<Reaction> store)
        {
            _store = store;
        }

        public ReactionQueryParams Params { get; set; }

        public override IQuery<Reaction> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (ReactionQueryParams)Convert.ChangeType(defaultParams, typeof(ReactionQueryParams));
            return this;
        }

        public override async Task<IPagedResults<Reaction>> ToList()
        {

            var builder = new ReactionQueryBuilder(this);
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

    #region "ReactionQueryParams"

    public class ReactionQueryParams
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

    #region "ReactionQueryBuilder"

    public class ReactionQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _reactionsTableName;

        private readonly ReactionQuery _query;

        public ReactionQueryBuilder(ReactionQuery query)
        {
            _query = query;
            _reactionsTableName = GetTableNameWithPrefix("Reactions");

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
            sb.Append("e.*");
            return sb.ToString();

        }

        string BuildTables()
        {

            var sb = new StringBuilder();

            sb.Append(_reactionsTableName)
                .Append(" e ");

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
