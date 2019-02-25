using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Schema;

namespace Plato.Internal.Repositories.Schema
{
    public interface IConstraintRepository
    {
        Task<IEnumerable<DbConstraint>> SelectConstraintsAsync();
    }

}
