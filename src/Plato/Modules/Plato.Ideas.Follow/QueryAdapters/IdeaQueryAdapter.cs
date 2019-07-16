using System;
using System.Text;
using Plato.Ideas.Models;
using Plato.Entities.Stores;
using Plato.Follows.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Ideas.Follow.QueryAdapters
{

    public class IdeaQueryAdapter : BaseQueryAdapterProvider<Idea> 
    {

        public override void BuildWhere(IQuery<Idea> query, StringBuilder builder)
        {

            // Ensure correct query type
            if (query.GetType() != typeof(EntityQuery<Idea>))
            {
                return;
            }

            // Convert to correct query type
            var q = (EntityQuery<Idea>)Convert.ChangeType(query, typeof(EntityQuery<Idea>));

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
                    .Append(FollowTypes.Idea.Name)
                    .Append("' AND ")
                    .Append(q.Params.FollowUserId.ToSqlString("f.CreatedUserId"))
                    .Append("))");

            }

        }

    }

}
