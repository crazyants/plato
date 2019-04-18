using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Search.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities
{
    public class EntitySearchQueries<TModel> : IFederatedQueryProvider<TModel> where TModel : class
    {

        protected readonly IFullTextQueryParser _fullTextQueryParser;

        public EntitySearchQueries(
            IFullTextQueryParser fullTextQueryParser)
        {
            _fullTextQueryParser = fullTextQueryParser;
        }

        public IEnumerable<string> GetQueries(IFederatedQueryContext<TModel> context)
        {

            // Ensure correct query type for federated query
            if (context.Query.GetType() != typeof(EntityQuery<TModel>))
            {
                return null;
            }

            return context.Query.Options.SearchType != SearchTypes.Tsql
                ? BuildFullTextQueries(context)
                : BuildSqlQueries(context);
        }

        List<string> BuildSqlQueries(IFederatedQueryContext<TModel> context)
        {

            // Entities
            // ----------------------

            var q1 = new StringBuilder();
            q1.Append("SELECT e.Id, 0 AS [Rank] FROM ")
                .Append("{prefix}_Entities")
                .Append(" e WHERE (");
            if (!string.IsNullOrEmpty(context.Where))
            {
                q1.Append("(").Append(context.Where).Append(") AND ");
            }
            q1.Append("(")
                .Append(context.Keywords.ToSqlString("e.Title", "Keywords"))
                .Append(" OR ")
                .Append(context.Keywords.ToSqlString("e.Message", "Keywords"))
                .Append("));");

            // Entity Replies
            // ----------------------

            var q2 = new StringBuilder();
            q2.Append("SELECT er.EntityId, 0 AS [Rank] FROM ")
                .Append("{prefix}_EntityReplies")
                .Append(" er INNER JOIN {prefix}_Entities ")
                .Append("e ON e.Id = er.EntityId ")
                .Append(" WHERE (");
            if (!string.IsNullOrEmpty(context.Where))
            {
                q2.Append("(").Append(context.Where).Append(") AND ");
            }
            q2.Append("(")
                .Append(context.Keywords.ToSqlString("er.Message", "Keywords"))
                .Append(")) GROUP BY er.EntityId");

            // Return queries
            return new List<string>()
            {
                q1.ToString(),
                q2.ToString()
            };
            
        }


        List<string> BuildFullTextQueries(IFederatedQueryContext<TModel> context)
        {

            // Parse keywords into valid full text query syntax
            var fullTextQuery = _fullTextQueryParser.ToFullTextSearchQuery(context.Keywords.Value);

            // Ensure parse was successful
            if (!String.IsNullOrEmpty(fullTextQuery))
            {
                fullTextQuery = fullTextQuery.Replace("'", "''");
            }

            var q1 = new StringBuilder();
            q1
                .Append("SELECT i.[Key], i.[Rank] ")
                .Append("FROM ")
                .Append("{prefix}_Entities")
                .Append(" e ")
                .Append("INNER JOIN ")
                .Append(context.Query.Options.SearchType.ToString().ToUpper())
                .Append("(")
                .Append("{prefix}_Entities")
                .Append(", *, '").Append(fullTextQuery).Append("'");
            if (context.Query.Options.MaxResults > 0)
                q1.Append(", ").Append(context.Query.Options.MaxResults.ToString());
            q1.Append(") AS i ON i.[Key] = e.Id WHERE ");
            if (!string.IsNullOrEmpty(context.Where))
                q1.Append("(").Append(context.Where).Append(") AND ");
            q1.Append("(e.Id IN (IsNull(i.[Key], 0)));");

            // Entity replies
            // ----------------------

            var q2 = new StringBuilder();
            q2
                .Append("SELECT er.EntityId, SUM(i.[Rank]) AS [Rank] ")
                .Append("FROM ")
                .Append("{prefix}_EntityReplies")
                .Append(" er ")
                .Append("INNER JOIN ")
                .Append(context.Query.Options.SearchType.ToString().ToUpper())
                .Append("(")
                .Append("{prefix}_EntityReplies")
                .Append(", *, '").Append(fullTextQuery).Append("'");
            if (context.Query.Options.MaxResults > 0)
                q2.Append(", ").Append(context.Query.Options.MaxResults.ToString());
            q2.Append(") i ON i.[Key] = er.Id ")
                .Append("INNER JOIN ")
                .Append("{prefix}_Entities")
                .Append(" e ON e.Id = er.EntityId ")
                .Append("WHERE ");
            if (!string.IsNullOrEmpty(context.Where))
                q1.Append("(").Append(context.Where).Append(") AND ");
            q2.Append("(er.Id IN (IsNull(i.[Key], 0)))")
                .Append("GROUP BY er.EntityId, i.[Rank];");

            // Return queries
            return new List<string>()
            {
                q1.ToString(),
                q2.ToString()
            };

        }

    }

}
