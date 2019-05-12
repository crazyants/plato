using System;
using System.Text;
using Plato.Categories.Stores;
using Plato.Discuss.Categories.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Discuss.Categories.Roles.QueryAdapters
{

    public class CategoryQueryAdapter : IQueryAdapterProvider<Category>
    {

        public string AdaptQuery(IQuery<Category> query)
        {

            // Ensure correct query type
            if (query.GetType() != typeof(CategoryQuery<Category>))
            {
                return string.Empty;
            }

            // Convert to correct query type
            var q = (CategoryQuery<Category>)Convert.ChangeType(query, typeof(CategoryQuery<Category>));
            
            // only return categories if the user belongs to one
            // or more roles associated with the category

            var sb = new StringBuilder();

            // Only apply role based security
            // checks if user id is 0 or above
            if (q.Params.UserId.Value > -1)
            {
                sb.Append("(c.Id IN (");
                if (q.Params.UserId.Value > 0)
                {
                    sb.Append("SELECT cr.CategoryId FROM {prefix}_CategoryRoles AS cr WITH (nolock) WHERE cr.RoleId IN (");
                    sb.Append("SELECT ur.RoleId FROM {prefix}_UserRoles AS ur WITH (nolock) WHERE ur.UserId = ");
                    sb.Append(q.Params.UserId.Value)
                        .Append(")");
                }
                else
                {
                    // anonymous user
                    sb.Append("SELECT cr.CategoryId FROM {prefix}_CategoryRoles AS cr WITH (nolock) WHERE (cr.RoleId = ");
                    sb.Append("(SELECT TOP 1 r.Id FROM {prefix}_Roles r WHERE r.[Name] = '")
                        .Append(DefaultRoles.Anonymous)
                        .Append("')");
                    sb.Append(")");
                }
                sb.Append("))");
            }

            return sb.ToString();

        }

    }


}
