using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Search.Abstractions;
using Plato.Internal.Stores.Abstractions.FederatedQueries;

namespace Plato.Entities.Tags
{

    public class EntityTagSearchQueries<TModel> : IFederatedQueryProvider<TModel> where TModel : class
    {

        protected readonly IFullTextQueryParser _fullTextQueryParser;

        public EntityTagSearchQueries(IFullTextQueryParser fullTextQueryParser)
        {
            _fullTextQueryParser = fullTextQueryParser;
        }

        public IEnumerable<string> Build(IQuery<TModel> query)
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

        IList<string> BuildSqlQueries(EntityQuery<TModel> query)
        {
            
            /*
                Produces the following federated query...
                -----------------
                SELECT et.EntityId, 0 FROM plato_Tags t
                INNER JOIN plato_EntityTags et ON et.TagId = t.Id
                INNER JOIN plato_Entities e ON e.Id = et.EntityId
                WHERE (t.[Name] LIKE '%percent') GROUP BY et.EntityId;     
             */

            var q1 = new StringBuilder();
            q1.Append("SELECT et.EntityId, 0 FROM {prefix}_Tags t ")
                .Append("INNER JOIN {prefix}_EntityTags et ON et.TagId = t.Id ")
                .Append("INNER JOIN {prefix}_Entities e ON e.Id = et.EntityId ")
                .Append(" WHERE (");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(")
                .Append(query.Params.Keywords.ToSqlString("t.[Name]", "Keywords"))
                .Append(" OR ")
                .Append(query.Params.Keywords.ToSqlString("t.[Description]", "Keywords"))
                .Append("));");

            // Return queries
            return new List<string>()
            {
                q1.ToString()
            };
            
        }
        
        IList<string> BuildFullTextQueries(EntityQuery<TModel> query)
        {
            
            // Parse keywords into valid full text query syntax
            var fullTextQuery = _fullTextQueryParser.ToFullTextSearchQuery(query.Params.Keywords.Value);

            // Ensure parse was successful
            if (!String.IsNullOrEmpty(fullTextQuery))
            {
                fullTextQuery = fullTextQuery.Replace("'", "''");
            }

            // Can be empty if only punctuation or stop words were entered
            if (string.IsNullOrEmpty(fullTextQuery))
            {
                return null;
            }

            /*
                Produces the following federated query...
                -----------------
                SELECT et.EntityId, SUM(i.[Rank])
                FROM plato_Tags t INNER JOIN 
                CONTAINSTABLE(plato_Tags, *, 'FORMSOF(INFLECTIONAL, percent)') AS i ON i.[Key] = t.Id 
                INNER JOIN plato_EntityTags et ON et.TagId = t.Id
                INNER JOIN plato_Entities e ON e.Id = et.EntityId
                WHERE (t.Id IN (IsNull(i.[Key], 0))) GROUP BY et.EntityId;             
             */

            var q1 = new StringBuilder();
            q1
                .Append("SELECT et.EntityId, SUM(i.[Rank]) ")
                .Append("FROM {prefix}_Tags t INNER JOIN ")
                .Append(query.Options.SearchType.ToString().ToUpper())
                .Append("({prefix}_Tags")
                .Append(", *, '").Append(fullTextQuery).Append("'");
            if (query.Options.MaxResults > 0)
                q1.Append(", ").Append(query.Options.MaxResults.ToString());
            q1.Append(") AS i ON i.[Key] = t.Id ")
                .Append("INNER JOIN {prefix}_EntityTags et ON et.TagId = t.Id ")
                .Append("INNER JOIN {prefix}_Entities e ON e.Id = et.EntityId ")
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
