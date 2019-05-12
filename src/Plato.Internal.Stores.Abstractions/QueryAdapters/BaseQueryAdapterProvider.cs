using System.Text;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions.QueryAdapters
{

    /// <summary>
    /// Provides extensibility for IQuery implementations.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class BaseQueryAdapterProvider<TModel> : IQueryAdapterProvider<TModel> where TModel : class
    {

        public virtual void BuildSelect(IQuery<TModel> query, StringBuilder builder)
        {
            return;
        }

        public virtual void BuildTables(IQuery<TModel> query, StringBuilder builder)
        {
            return;
        }

        public virtual void BuildWhere(IQuery<TModel> query, StringBuilder builder)
        {
            return;
        }

    }

}
