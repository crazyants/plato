using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Plato.Models.User;

namespace Plato.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> InsertUpdate(T entity);

        Task<T> SelectById(int Id);


        IEnumerable<T> SelectPaged(int pageIndex, int pageSize, object options);

        bool Dlete(int Id);
    }
}
