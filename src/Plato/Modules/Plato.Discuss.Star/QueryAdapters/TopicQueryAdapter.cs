using System;
using System.Text;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Discuss.Star.QueryAdapters
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
                    .Append("{prefix}_Stars s")
                    .Append(" WHERE (")
                    .Append(q.Params.StarUserId.ToSqlString("s.CreatedUserId"))
                    .Append("))");
            }
            
        }

    }

}
