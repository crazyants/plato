using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Moderation.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Moderation.Stores
{

    #region "ModeratorQuery"

    public class ModeratorQuery : DefaultQuery<Moderator>
    {

        private readonly IStore<Moderator> _store;

        public ModeratorQuery(IStore<Moderator> store)
        {
            _store = store;
        }

        public ModeratorQueryParams Params { get; set; }

        public override IQuery<Moderator> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (ModeratorQueryParams)Convert.ChangeType(defaultParams, typeof(ModeratorQueryParams));
            return this;
        }

        public override async Task<IPagedResults<Moderator>> ToList()
        {

            var builder = new ModeratorQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var keywords = Params?.Keywords?.Value ?? string.Empty;

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                keywords
            );

            return data;
        }
        
    }

    #endregion

    #region "ModeratorQueryParams"

    public class ModeratorQueryParams
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

    #region "ModeratorQueryBuilder"

    public class ModeratorQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _moderatorsTableName;
        private readonly string _usersTableName;

        private readonly ModeratorQuery _query;

        public ModeratorQueryBuilder(ModeratorQuery query)
        {
            _query = query;
            _moderatorsTableName = GetTableNameWithPrefix("Moderators");
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
            sb.Append("SELECT COUNT(m.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("m.*, ")
                .Append("u.UserName,")
                .Append("u.DisplayName,")
                .Append("u.FirstName,")
                .Append("u.LastName,")
                .Append("u.Alias");

            return sb.ToString();

        }

        string BuildTables()
        {

            var sb = new StringBuilder();

            sb.Append(_moderatorsTableName)
                .Append(" m ");

            // join users to obtain simple moderator details
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" u ON m.UserId = u.Id ");
            
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
                sb.Append(_query.Params.Id.ToSqlString("m.Id"));
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
                : "m." + columnName;
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
