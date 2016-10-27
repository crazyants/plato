using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions.Stores
{
    public interface IStorePaged<T> where T : class 
    {
        
        Task<T> Get(int pageIndex, int pageSize, object args);


    }
}
