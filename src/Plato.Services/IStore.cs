using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Services
{
    public interface IStore<T> where T : class
    {

        Task<T> Get();

        Task<T> Get(int id);

        Task<T> Save(T model);

    }
}
