using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions.Query
{


    public interface IQuery
    {

        IQuery Page(int pageIndex, int pageSize);

        IQuery Select<T>(Action<T> configure) where T : new();

        Task<IEnumerable<T>> ToList<T>() where T : class;

    }


}
