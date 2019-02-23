using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Schema;
using Plato.Internal.Stores.Abstractions.Schema;

namespace Plato.Internal.Stores.Extensions
{
    public static class ConstraintStoreExtensions
    {

        public static async Task<DbConstraint> GetPrimaryKeyConstraint(
            this IConstraintStore store,
            string tableName)
        {
            var constraints = await store.SelectConstraintsAsync();
            return constraints.FirstOrDefault(c => c.TableName == tableName && c.ConstraintType == ConstraintTypes.PrimaryKey);
        }

    }
}
