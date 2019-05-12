using System.Text;
using Plato.Entities.Stores;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Categories.Roles.QueryAdapters
{
    public class EntityQueryAdapter : IQueryAdapterProvider<EntityQueryParams> 
    {

        public string AdaptQuery(EntityQueryParams queryParams)
        {
         
            if (queryParams == null)
            {
                return string.Empty;
            }

            // only return entities from categories if the user 
            // belongs to one or more roles associated with the category

            var sb = new StringBuilder();

            // Only apply role based security
            // checks if user id is 0 or above
            if (queryParams.UserId.Value > -1)
            {
                sb.Append("(e.CategoryId IN (");
                if (queryParams.UserId.Value > 0)
                {
                    sb.Append("SELECT cr.CategoryId FROM {prefix}_CategoryRoles AS cr WITH (nolock) WHERE cr.RoleId IN (");
                    sb.Append("SELECT ur.RoleId FROM {prefix}_UserRoles AS ur WITH (nolock) WHERE ur.UserId = ");
                    sb.Append(queryParams.UserId.Value)
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
