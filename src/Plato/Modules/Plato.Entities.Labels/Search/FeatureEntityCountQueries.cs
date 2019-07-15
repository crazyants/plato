using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Search.Abstractions;
using Plato.Internal.Stores.Abstractions.FederatedQueries;

namespace Plato.Entities.Labels.Search
{

    public class FeatureEntityCountQueries<TModel> : IFederatedQueryProvider<TModel> where TModel : class
    {

        protected readonly IFullTextQueryParser _fullTextQueryParser;

        public FeatureEntityCountQueries(IFullTextQueryParser fullTextQueryParser)
        {
            _fullTextQueryParser = fullTextQueryParser;
        }

        public IEnumerable<string> Build(IQuery<TModel> query)
        {

            // Ensure correct query type for federated query
            if (query.GetType() != typeof(FeatureEntityCountQuery<TModel>))
            {
                return null;
            }

            // Convert to correct query type
            var entityQuery = (FeatureEntityCountQuery<TModel>)Convert.ChangeType(query, typeof(FeatureEntityCountQuery<TModel>));

            return query.Options.SearchType != SearchTypes.Tsql
                ? BuildFullTextQueries(entityQuery)
                : BuildSqlQueries(entityQuery);
        }

        List<string> BuildSqlQueries(FeatureEntityCountQuery<TModel> query)
        {

            /*
                 Produces the following federated query...
                 -----------------
                 SELECT el.EntityId, 0 FROM plato_Labels l
                 INNER JOIN plato_EntityLabels el ON el.LabelId = l.Id
                 INNER JOIN plato_Entities e ON e.Id = el.EntityId
                 WHERE (l.[Name] LIKE '%percent') GROUP BY el.EntityId;     
             */

            var q1 = new StringBuilder();
            q1.Append("SELECT el.EntityId, 0 FROM {prefix}_Labels l ")
                .Append("INNER JOIN {prefix}_EntityLabels el ON el.LabelId = l.Id ")
                .Append("INNER JOIN {prefix}_Entities e ON e.Id = el.EntityId ")
                .Append("WHERE (");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(")
                .Append(query.Params.Keywords.ToSqlString("l.[Name]", "Keywords"))
                .Append(" OR ")
                .Append(query.Params.Keywords.ToSqlString("l.[Description]", "Keywords"))
                .Append("));");

            // Return queries
            return new List<string>()
            {
                q1.ToString()
            };

        }

        List<string> BuildFullTextQueries(FeatureEntityCountQuery<TModel> query)
        {

            // Parse keywords into valid full text query syntax
            var fullTextQuery = _fullTextQueryParser.ToFullTextSearchQuery(query.Params.Keywords.Value);

            // Ensure parse was successful
            if (!String.IsNullOrEmpty(fullTextQuery))
            {
                fullTextQuery = fullTextQuery.Replace("'", "''");
            }

            // Can be empty if only puntutaton or stop words were entered
            if (string.IsNullOrEmpty(fullTextQuery))
            {
                return null;
            }

            /*
                Produces the following federated query...
                -----------------
                SELECT el.EntityId, SUM(i.[Rank]) AS [Rank] 
                FROM plato_Labels l INNER JOIN 
                CONTAINSTABLE(plato_Labels, *, 'FORMSOF(INFLECTIONAL, creative)') AS i ON i.[Key] = l.Id 
                INNER JOIN plato_EntityLabels el ON el.LabelId = l.Id
                INNER JOIN plato_Entities e ON e.Id = el.EntityId
                WHERE (l.Id IN (IsNull(i.[Key], 0))) GROUP BY el.EntityId;
             */

            var q1 = new StringBuilder();
            q1
                .Append("SELECT el.EntityId, SUM(i.[Rank]) ")
                .Append("FROM ")
                .Append("{prefix}_Labels")
                .Append(" l ")
                .Append("INNER JOIN ")
                .Append(query.Options.SearchType.ToString().ToUpper())
                .Append("(")
                .Append("{prefix}_Labels")
                .Append(", *, '").Append(fullTextQuery).Append("'");
            if (query.Options.MaxResults > 0)
                q1.Append(", ").Append(query.Options.MaxResults.ToString());
            q1.Append(") AS i ON i.[Key] = l.Id ")
                .Append("INNER JOIN {prefix}_EntityLabels el ON el.LabelId = l.Id ")
                .Append("INNER JOIN plato_Entities e ON e.Id = el.EntityId ")
                .Append("WHERE ");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(l.Id IN (IsNull(i.[Key], 0))) GROUP BY el.EntityId;");

            // Return queries
            return new List<string>()
            {
                q1.ToString()
            };

        }

    }

}
