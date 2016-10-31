using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Plato.Abstractions.Collections
{
    public interface IPagedResults<T> where T : class
    {

        int Total { get; set; }

    }
}
