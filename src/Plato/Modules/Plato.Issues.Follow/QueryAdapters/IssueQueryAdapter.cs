using System;
using System.Text;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using Plato.Follows.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Issues.Follow.QueryAdapters
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
            // FollowUserId
            // Only return entities if an entry exists within the Follows table
            // -----------------

            if (q.Params.FollowUserId.Value > 0)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" AND ");
                }

                builder.Append(" e.Id IN (")
                    .Append("SELECT ThingId FROM {prefix}_Follows f WHERE (f.[Name] = '")
                    .Append(FollowTypes.Issue.Name)
                    .Append("' AND ")
                    .Append(q.Params.FollowUserId.ToSqlString("f.CreatedUserId"))
                    .Append("))");

            }

        }

    }

}
