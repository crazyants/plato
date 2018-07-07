using System;
using Plato.Internal.Repositories;
using Plato.Email.Models;
using System.Threading.Tasks;

namespace Plato.Email.Repositories
{

    public interface IEmailRepository<T> : IRepository<T> where T : class
    {

    }


}
