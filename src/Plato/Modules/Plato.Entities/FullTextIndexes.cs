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
    public class FullTextIndexes : IFullTextIndexProvider
    {

        public readonly static FullTextIndex Title = 
            new FullTextIndex("Entities", "Title", 1033, 30);

        public readonly static FullTextIndex Message =
            new FullTextIndex("Entities", "Message", 1033, 30);

        public IEnumerable<FullTextIndex> GetIndexes()
        {
            return new[]
            {
                Title,
                Message
            };
        }

        //public async Task<IEnumerable<string>> CreateIndexAsync()
        //{

        //    var errors = new List<string>();
        //    var tableName = _shellSettings.TablePrefix + TableName;
        //    var pkConstraint = await _constraintStore.GetPrimaryKeyConstraint(tableName);
        //    if (pkConstraint == null)
        //    {
        //        errors.Add($"Could not find a primary key constraint for table {tableName}");
        //    }

        //    return errors;

        //}

    }
}
