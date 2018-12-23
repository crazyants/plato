using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Stores
{

    #region "EntityReactionQuery"

    public class EntityReactionsQuery : DefaultQuery<EntityReaction>
    {

        private readonly IStore<EntityReaction> _store;

        public EntityReactionsQuery(IStore<EntityReaction> store)
        {
            _store = store;
        }

        public EntityReactionsQueryParams Params { get; set; }

        public override IQuery<EntityReaction> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityReactionsQueryParams)Convert.ChangeType(defaultParams, typeof(EntityReactionsQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EntityReaction>> ToList()
        {

            var builder = new EntityReactionsQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.ReactionName.Value
            );

            return data;
        }


    }

    #endregion

    #region "EntityReactionsQueryParams"

    public class EntityReactionsQueryParams
    {


        private WhereInt _id;
        private WhereInt _entityId;
        private WhereString _reactionName;


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


        public WhereString ReactionName
        {
            get => _reactionName ?? (_reactionName = new WhereString());
            set => _reactionName = value;
        }


    }

    #endregion

    #region "EntityReactionsQueryBuilder"

    public class EntityReactionsQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _entityReactionsTableName;
        private readonly string _usersTableName;

        private readonly EntityReactionsQuery _query;

        public EntityReactionsQueryBuilder(EntityReactionsQuery query)
        {
            _query = query;
            _entityReactionsTableName = GetTableNameWithPrefix("EntityReactions");
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
            sb.Append("SELECT COUNT(er.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("er.*, ")
                .Append("u.UserName,")
                .Append("u.DisplayName,")
                .Append("u.Alias,")
                .Append("u.PhotoUrl,")
                .Append("u.PhotoColor");
            return sb.ToString();

        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_entityReactionsTableName)
                .Append(" er ");

            // join user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" u ON er.CreatedUserId = u.Id ");
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

            // Id
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("er.Id"));
            }

            // ReactionName
            if (!String.IsNullOrEmpty(_query.Params.ReactionName.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ReactionName.Operator);
                sb.Append(_query.Params.ReactionName.ToSqlString("ReactionName", "Keywords"));
            }
            
            // EntityId
            if (_query.Params.EntityId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("er.EntityId"));
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
                : "er." + columnName;
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
