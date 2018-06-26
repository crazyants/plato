using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Repositories;

namespace Plato.Entities.Repositories
{
    public interface IEntityRepository<T> : IRepository<T> where T : class
    {

    }

}
