using System;
using System.Collections.Generic;
using System.Data;
using Plato.Abstractions.Extensions;

namespace Plato.Abstractions.Collections
{
    public class PagedResults<T> : List<T>,
        IPagedResults<T> where T : class
    {
        public int Total { get; set; }


        public void PopulateTotal(IDataReader reader)
        {
            if (reader.ColumnIsNotNull(0))
                Total = Convert.ToInt32(reader[0]);
        }
    }
}