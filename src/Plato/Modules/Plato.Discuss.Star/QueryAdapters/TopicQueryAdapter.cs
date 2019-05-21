using System;
using System.Text;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Security.Abstractions;
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
            
            //if (q.Params.UserId.Value > -1)
            //{

            //    if (!string.IsNullOrEmpty(builder.ToString()))
            //    {
            //        builder.Append(" AND ");
            //    }

            //    builder.Append("(e.CategoryId = 0 OR e.CategoryId IN (");
            //    if (q.Params.UserId.Value > 0)
            //    {
            //        builder.Append("SELECT cr.CategoryId FROM {prefix}_CategoryRoles AS cr WITH (nolock) WHERE cr.RoleId IN (");
            //        builder.Append("SELECT ur.RoleId FROM {prefix}_UserRoles AS ur WITH (nolock) WHERE ur.UserId = ");
            //        builder.Append(q.Params.UserId.Value)
            //            .Append(")");
            //    }
            //    else
            //    {
            //        // anonymous user
            //        builder.Append("SELECT cr.CategoryId FROM {prefix}_CategoryRoles AS cr WITH (nolock) WHERE (cr.RoleId = ");
            //        builder.Append("(SELECT r.Id FROM {prefix}_Roles r WHERE r.[Name] = '")
            //            .Append(DefaultRoles.Anonymous)
            //            .Append("')");
            //        builder.Append(")");
            //    }
            //    builder.Append("))");
            //}
            
        }

    }

}
