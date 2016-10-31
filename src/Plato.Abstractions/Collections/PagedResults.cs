using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions.Collections
{
    public class PagedResults<T> : List<T>,
        IPagedResults<T> where T : class
    {
        public int Total { get; set; }

    }
}
