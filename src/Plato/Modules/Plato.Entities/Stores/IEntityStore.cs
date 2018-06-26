using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    public interface IEntityStore<T> : IStore<T> where T : class
    {

    }


}
