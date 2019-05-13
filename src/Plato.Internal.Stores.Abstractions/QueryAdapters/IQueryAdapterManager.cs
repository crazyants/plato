using System.Collections.Generic;
using System.Text;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions.QueryAdapters
{

    public interface IQueryAdapterManager<TModel> where TModel : class
    {

        void BuildSelect(IQuery<TModel> query, StringBuilder builder);

        void BuildTables(IQuery<TModel> query, StringBuilder builder);

        void BuildWhere(IQuery<TModel> query, StringBuilder builder);

    }
    
}
