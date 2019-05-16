using System;
using System.Text;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Discuss.Categories.Roles.QueryAdapters
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
            
            // only return entities from categories if the user 
            // belongs to one or more roles associated with the category
            // Only apply role based security if user id is 0 or above

            if (q.Params.UserId.Value > -1)
            {

                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" AND ");
                }

                builder.Append("(e.CategoryId = 0 OR e.CategoryId IN (");
                if (q.Params.UserId.Value > 0)
                {
                    builder.Append("SELECT cr.CategoryId FROM {prefix}_CategoryRoles AS cr WITH (nolock) WHERE cr.RoleId IN (");
                    builder.Append("SELECT ur.RoleId FROM {prefix}_UserRoles AS ur WITH (nolock) WHERE ur.UserId = ");
                    builder.Append(q.Params.UserId.Value)
                        .Append(")");
                }
                else
                {
                    // anonymous user
                    builder.Append("SELECT cr.CategoryId FROM {prefix}_CategoryRoles AS cr WITH (nolock) WHERE (cr.RoleId = ");
                    builder.Append("(SELECT r.Id FROM {prefix}_Roles r WHERE r.[Name] = '")
                        .Append(DefaultRoles.Anonymous)
                        .Append("')");
                    builder.Append(")");
                }
                builder.Append("))");
            }
            
        }

    }

}
