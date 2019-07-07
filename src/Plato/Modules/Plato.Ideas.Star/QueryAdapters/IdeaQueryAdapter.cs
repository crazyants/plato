using System;
using System.Text;
using Plato.Ideas.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Ideas.Star.QueryAdapters
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
                    .Append(StarTypes.Idea.Name)
                    .Append("' AND ")
                    .Append(q.Params.StarUserId.ToSqlString("s.CreatedUserId"))
                    .Append("))");
            }
            
        }

    }

}
