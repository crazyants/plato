using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Plato.Repositories.Models;

namespace Plato.Repositories
{
    public interface IRepository<T> where T : class
    {
        int InsertUpdate(T entity);

        T Select(int id);

        IEnumerable<T> SelectPaged(int pageIndex, int pageSize, object options);

        bool Dlete(int id);
    }
}
