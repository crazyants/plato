using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Schema;
using Plato.Internal.Models.Shell;
using Plato.Internal.Repositories.Schema;
using Plato.Internal.Search.Abstractions;
using Plato.Internal.Stores.Abstractions.Schema;
using Plato.Internal.Stores.Extensions;

namespace Plato.Entities
{
    public class FullTextIndexProvider : IFullTextIndexProvider
    {

        private readonly IShellSettings _shellSettings;
        private readonly IConstraintStore _constraintStore;
        
        public FullTextIndexProvider(
            IConstraintStore constraintStore, 
            IShellSettings shellSettings)
        {
            _constraintStore = constraintStore;
            _shellSettings = shellSettings;
        }

        public const string TableName = "Entities";

        public async Task<IEnumerable<string>> CreateIndexAsync()
        {

            var errors = new List<string>();
            var tableName = _shellSettings.TablePrefix + TableName;
            var pkConstraint = await _constraintStore.GetPrimaryKeyConstraint(tableName);
            if (pkConstraint == null)
            {
                errors.Add($"Could not find a primary key constraint for table {tableName}");
            }

            return errors;

        }

    }
}
