using System.Collections.Generic;
using System.Data;

namespace Plato.Abstractions.Collections
{
    public interface IPagedResults<T> : IList<T> where T : class
    {
        int Total { get; set; }

        void PopulateTotal(IDataReader reader);
    }
}