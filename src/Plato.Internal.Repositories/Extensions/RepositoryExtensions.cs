using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Extensions
{
    public static class RepositoryExtensions
    {
        public static IEnumerable<T> QueryAsync<T>(
            this IRepository<T> repository) where T : class
        {

            throw new NotImplementedException();

        }

    }
}
