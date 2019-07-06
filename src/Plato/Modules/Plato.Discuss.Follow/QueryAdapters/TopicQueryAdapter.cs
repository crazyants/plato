using System;
using System.Text;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Follows.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Discuss.Follow.QueryAdapters
{

    public class TopicQueryAdapter : BaseQueryAdapterProvider<Topic> 
    {

        public override void BuildWhere(IQuery<Topic> query, StringBuilder builder)
        {

            // Ensure correct query type
            if (query.GetType() != typeof(EntityQuery<Topic>))
            {
                return;
            }

            // Convert to correct query type
            var q = (EntityQuery<Topic>)Convert.ChangeType(query, typeof(EntityQuery<Topic>));

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
                    .Append(FollowTypes.Topic.Name)
                    .Append("' AND ")
                    .Append(q.Params.FollowUserId.ToSqlString("f.CreatedUserId"))
                    .Append("))");

            }

        }

    }

}
