using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Plato.Data.Query;

namespace Plato.Repositories.Roles
{
    public interface IRoleRepository<T> : IRepository<T> where T : class
    {
      
        Task<T> SelectByNameAsync(string name);
        
        IQuery Query { get; }

    }
}
