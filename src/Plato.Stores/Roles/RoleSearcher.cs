using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Plato.Repositories;
using Plato.Repositories.Roles;

namespace Plato.Stores.Roles
{
    public static class RoleSearcher
    {

        public static Task<IEnumerable<T>> QueryAsync<T>(
            this IRoleRepository<T> roleRepository, 
            Expression<Func<T, bool>> predicate) where T : class
        {

            throw new NotImplementedException();
            //return roleRepository.QueryAsync(SimpleMapper<T>(predicate));
       
        }

        private static string SimpleMapper<T>(
            Expression<Func<T, bool>> predicate)
        {

            dynamic operation = predicate.Body;
            dynamic left = operation.Left;
            dynamic right = operation.Right;

            var ops = new Dictionary<ExpressionType, string>();
            ops.Add(ExpressionType.Equal, "=");
            ops.Add(ExpressionType.GreaterThan, ">");

            var sql = String.Format("SELECT * FROM {0} WHERE {1} {2} {3}",
                typeof(T),
                left.Member.Name,
                ops[operation.NodeType],
                right.Value);

            return sql;

        }

    }
}
