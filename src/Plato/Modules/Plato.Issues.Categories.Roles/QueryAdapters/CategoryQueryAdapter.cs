using System;
using System.Text;
using Plato.Categories.Stores;
using Plato.Issues.Categories.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Issues.Categories.Roles.QueryAdapters
{

    public class CategoryQueryAdapter : BaseQueryAdapterProvider<Category>
    {

        public override void BuildWhere(IQuery<Category> query, StringBuilder builder)
        {

            // Ensure correct query type
            if (query.GetType() != typeof(CategoryQuery<Category>))
            {
                return;
            }

            // Convert to correct query type
            var q = (CategoryQuery<Category>) Convert.ChangeType(query, typeof(CategoryQuery<Category>));
            
            // only return categories if the user belongs to one
            // or more roles associated with the category
            // Only apply role based security
            // checks if user id is 0 or above

            if (q.Params.UserId.Value > -1)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" AND ");
                }

                builder.Append("(c.Id IN (");
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
                    builder.Append("(SELECT TOP 1 r.Id FROM {prefix}_Roles r WHERE r.[Name] = '")
                        .Append(DefaultRoles.Anonymous)
                        .Append("')");
                    builder.Append(")");
                }
                builder.Append("))");

            }
            
        }

    }


}
