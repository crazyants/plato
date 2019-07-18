using System;
using System.Text;
using Plato.Docs.Models;
using Plato.Entities.Stores;
using Plato.Follows.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Docs.Follow.QueryAdapters
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
                    .Append(FollowTypes.Doc.Name)
                    .Append("' AND ")
                    .Append(q.Params.FollowUserId.ToSqlString("f.CreatedUserId"))
                    .Append("))");

            }

        }

    }

}
