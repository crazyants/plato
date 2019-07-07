using System;
using System.Text;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Issues.Star.QueryAdapters
{

    public class IssueQueryAdapter : BaseQueryAdapterProvider<Issue> 
    {

        public override void BuildWhere(IQuery<Issue> query, StringBuilder builder)
        {

            // Ensure correct query type
            if (query.GetType() != typeof(EntityQuery<Issue>))
            {
                return;
            }

            // Convert to correct query type
            var q = (EntityQuery<Issue>)Convert.ChangeType(query, typeof(EntityQuery<Issue>));
            
            // -----------------
            // StarUserId
            // Only return entities if the user has starred the entity within the stars table
            // -----------------

            if (q.Params.StarUserId.Value > 0)
            {

                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" AND ");
                }

                builder.Append("e.Id IN (")
                    .Append("SELECT ThingId FROM ")
                    .Append("{prefix}_Stars s WHERE (s.[Name] = '")
                    .Append(StarTypes.Issue.Name)
                    .Append("' AND ")
                    .Append(q.Params.StarUserId.ToSqlString("s.CreatedUserId"))
                    .Append("))");
            }
            
        }

    }

}
