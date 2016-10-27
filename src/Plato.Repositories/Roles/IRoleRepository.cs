using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Plato.Repositories.Roles
{
    public interface IRoleRepository<T> : IRepository<T> where T : class
    {
      
        Task<T> SelectByNameAsync(string name);
        
        Task<IEnumerable<T>> QueryAsync(string sql);


    }
}
