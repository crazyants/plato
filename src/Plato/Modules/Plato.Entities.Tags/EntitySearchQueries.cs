using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Search.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Tags
{
    public class EntityTagSearchQueries<TModel> : IFederatedQueryProvider<TModel> where TModel : class
    {

        protected readonly IFullTextQueryParser _fullTextQueryParser;

        public EntityTagSearchQueries(IFullTextQueryParser fullTextQueryParser)
        {
            _fullTextQueryParser = fullTextQueryParser;
        }

        public IEnumerable<string> GetQueries(IQuery<TModel> query)
        {

            // Ensure correct query type for federated query
            if (query.GetType() != typeof(EntityQuery<TModel>))
            {
                return null;
            }
            
            // Convert to correct query type
            var entityQuery = (EntityQuery<TModel>)Convert.ChangeType(query, typeof(EntityQuery<TModel>));
            
            return query.Options.SearchType != SearchTypes.Tsql
                ? BuildFullTextQueries(entityQuery)
                : BuildSqlQueries(entityQuery);
        }

        List<string> BuildSqlQueries(EntityQuery<TModel> query)
        {
            
            // Entities
            // ----------------------

            var q1 = new StringBuilder();
            q1.Append("SELECT e.Id, 0 AS [Rank] FROM ")
                .Append("{prefix}_Entities")
                .Append(" e WHERE (");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(")
                .Append(query.Params.Keywords.ToSqlString("e.Title", "Keywords"))
                .Append(" OR ")
                .Append(query.Params.Keywords.ToSqlString("e.Message", "Keywords"))
                .Append("));");

            // Entity Replies
            // ----------------------

            var q2 = new StringBuilder();
            q2.Append("SELECT er.EntityId, 0 AS [Rank] FROM ")
                .Append("{prefix}_EntityReplies")
                .Append(" er INNER JOIN {prefix}_Entities ")
                .Append("e ON e.Id = er.EntityId ")
                .Append(" WHERE (");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q2.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q2.Append("(")
                .Append(query.Params.Keywords.ToSqlString("er.Message", "Keywords"))
                .Append(")) GROUP BY er.EntityId");

            // Return queries
            return new List<string>()
            {
                q1.ToString(),
                q2.ToString()
            };
            
        }


        List<string> BuildFullTextQueries(EntityQuery<TModel> query)
        {
            
            // Parse keywords into valid full text query syntax
            var fullTextQuery = _fullTextQueryParser.ToFullTextSearchQuery(query.Params.Keywords.Value);

            // Ensure parse was successful
            if (!String.IsNullOrEmpty(fullTextQuery))
            {
                fullTextQuery = fullTextQuery.Replace("'", "''");
            }

            /*                
                SELECT et.EntityId, SUM(i.[Rank]) AS [Rank] 
                FROM plato_Tags t INNER JOIN 
                CONTAINSTABLE(plato_Tags, *, 'FORMSOF(INFLECTIONAL, percent)') AS i ON i.[Key] = t.Id 
                INNER JOIN plato_EntityTags et ON et.TagId = t.Id
                WHERE (t.Id IN (IsNull(i.[Key], 0))) GROUP BY et.EntityId;
             *
             */
             
            var q1 = new StringBuilder();
            q1
                .Append("SELECT et.EntityId, SUM(i.[Rank]) ")
                .Append("FROM ")
                .Append("{prefix}_Tags")
                .Append(" t ")
                .Append("INNER JOIN ")
                .Append(query.Options.SearchType.ToString().ToUpper())
                .Append("(")
                .Append("{prefix}_Tags")
                .Append(", *, '").Append(fullTextQuery).Append("'");
            if (query.Options.MaxResults > 0)
                q1.Append(", ").Append(query.Options.MaxResults.ToString());
            q1.Append(") AS i ON i.[Key] = t.Id ")
                .Append("INNER JOIN {prefix}_EntityTags et ON et.TagId = t.Id ")
                .Append("WHERE ");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(t.Id IN (IsNull(i.[Key], 0))) GROUP BY et.EntityId;");

            // Return queries
            return new List<string>()
            {
                q1.ToString()
            };

        }

    }

}
