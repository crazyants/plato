using System;
using System.Text;
using Plato.Docs.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Docs.Star.QueryAdapters
{

    public class DocQueryAdapter : BaseQueryAdapterProvider<Doc> 
    {

        public override void BuildWhere(IQuery<Doc> query, StringBuilder builder)
        {

            // Ensure correct query type
            if (query.GetType() != typeof(EntityQuery<Doc>))
            {
                return;
            }

            // Convert to correct query type
            var q = (EntityQuery<Doc>)Convert.ChangeType(query, typeof(EntityQuery<Doc>));
            
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
                    .Append(StarTypes.Doc.Name)
                    .Append("' AND ")
                    .Append(q.Params.StarUserId.ToSqlString("s.CreatedUserId"))
                    .Append("))");
            }
            
        }

    }

}
