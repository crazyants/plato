using System.Text;
using Plato.Entities.Stores;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Categories.QueryAdapters
{
    public class EntityQueryParamAdapter : IQueryAdapterProvider<EntityQueryParams> 
    {

        public string AdaptQuery(EntityQueryParams queryParams)
        {
         
            if (queryParams == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            if (queryParams.CategoryId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(queryParams.CategoryId.Operator);
                sb.Append(queryParams.CategoryId.ToSqlString("e.CategoryId"));
            }

            return sb.ToString();

        }

    }

}
