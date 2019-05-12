using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions.QueryAdapters
{
    public interface IQueryAdapterManager<TModel> where TModel : class
    {

        IEnumerable<string> GetAdaptations(IQuery<TModel> query);

    }
    
}
