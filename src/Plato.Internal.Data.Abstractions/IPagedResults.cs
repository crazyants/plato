using System.Data;
using System.Collections.Generic;

namespace Plato.Internal.Data.Abstractions
{
    public interface IPagedResults<T> where T : class
    {
        int Total { get; set; }

        IList<T> Data { get; set; }

        void PopulateTotal(IDataReader reader);

    }

}