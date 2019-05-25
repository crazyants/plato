using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Stores.Abstractions;


namespace Plato.Internal.Stores.Roles
{

    #region "RoleQuery"

    public class RoleQuery : DefaultQuery<Role>
    {

        private readonly IStore2<Role> _store;

        public RoleQuery(IStore2<Role> store)
        {
            _store = store;
        }

        public RoleQueryParams Params { get; set; }

        public override IQuery<Role> Select<TParams>(Action<TParams> configure)
        {
            var defaultParams = new TParams();
            configure(defaultParams);
            Params = (RoleQueryParams)Convert.ChangeType(defaultParams, typeof(RoleQueryParams));
            return this;
        }
        
        public override async Task<IPagedResults<Role>> ToList()
        {

            var builder = new RoleQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var id = Params.Id.Value;
            var keywords = Params.Keywords.Value ?? string.Empty;

            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("Id", DbType.Int32, id),
                new DbParam("Keywords", DbType.String, keywords)
            });

        }

    }

    #endregion

    #region "RoleQueryParams"

    public class RoleQueryParams
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

    #region "RoleQueryBuilder"

    public class RoleQueryBuilder : IQueryBuilder
    {

        #region "Constructor"

        private readonly string _tableName;
        private const string TableName = "Roles";

        private readonly RoleQuery _query;

        public RoleQueryBuilder(RoleQuery query)
        {
            _query = query;
            _tableName = !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + TableName
                : TableName;
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var tablePrefix = _query.Options.TablePrefix;
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM ").Append(_tableName);
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
            sb.Append("SELECT COUNT(Id) FROM ").Append(_tableName);
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"
        
        private string BuildWhereClause()
        {
            var sb = new StringBuilder();
            
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("Id"));
            }

            if (!string.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("RoleName", "Keywords"));
            }
            
            return sb.ToString();
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in _query.SortColumns)
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

        #endregion

    }

    #endregion

}
