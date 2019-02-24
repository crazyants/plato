using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Search.Abstractions;
using Plato.Internal.Stores.Abstractions.Schema;
using Plato.Internal.Stores.Extensions;
using Plato.Search.Commands;

namespace Plato.Search.Services
{
       
    public class FullTextCatalogManager : IFullTextCatalogManager
    {

        private readonly IConstraintStore _constraintStore;
        private readonly IFullTextCatalogCommand<SchemaFullTextCatalog> _fullTextCatalogCommand;
        private readonly IFullTextIndexCommand<SchemaFullTextIndex> _fullTextIndexCommand;
        private readonly IFullTextIndexManager _fullTextIndexManager;
        private readonly IShellSettings _shellSettings;

        public FullTextCatalogManager(
            IFullTextCatalogCommand<SchemaFullTextCatalog> fullTextCatalogCommand,
            IFullTextIndexCommand<SchemaFullTextIndex> fullTextIndexCommand, 
            IFullTextIndexManager fullTextIndexManager,
            IShellSettings shellSettings,
            IConstraintStore constraintStore)
        {
            _fullTextCatalogCommand = fullTextCatalogCommand;
            _fullTextIndexCommand = fullTextIndexCommand;
            _fullTextIndexManager = fullTextIndexManager;
            _shellSettings = shellSettings;
            _constraintStore = constraintStore;
        }

        public async Task<ICommandResultBase> CreateCatalogAsync()
        {

            var result = new CommandResultBase();
            
            // First create the catalog
            var catalogName = GetCatalogName();
            var createCatalog = await _fullTextCatalogCommand.CreateAsync(new SchemaFullTextCatalog()
            {
                Name = catalogName
            });

            if (!createCatalog.Succeeded)
            {
                return result.Failed(createCatalog.Errors.ToArray());
            }

            // Next create the indexes from all provided indexes
            var indexes = _fullTextIndexManager.GetIndexes();
            foreach (var index in indexes)
            {

                // Get table name with prefix
                var tableName = GetTableName(index.TableName);

                // Get primary key constraint for table
                var primaryKey = await _constraintStore.GetPrimaryKeyConstraint(tableName);
                if (primaryKey == null)
                {
                    return result.Failed($"Could not find a primary key constraint for table {tableName}");
                }

                var schemaIndex = new SchemaFullTextIndex()
                {
                    PrimaryKeyName = primaryKey.ConstraintName,
                    TableName = index.TableName,
                    ColumnNames = index.ColumnNames,
                    LanguageCode = index.LanguageCode,
                    FillFactor = index.FillFactor,
                    CatalogName = catalogName
                };

                // Create the index
                var createIndex = await _fullTextIndexCommand.CreateAsync(schemaIndex);

                if (!createIndex.Succeeded)
                {
                    return result.Failed(createIndex.Errors.ToArray());
                }
            }

            return result.Success();

        }

        public async Task<ICommandResultBase> DropCatalogAsync()
        {

            var result = new CommandResultBase();

            // First drop all indexes on registered tables
            var indexes = _fullTextIndexManager.GetIndexes();
            foreach (var index in indexes)
            {
                var deleteIndex = await _fullTextIndexCommand.DeleteAsync(new SchemaFullTextIndex()
                {
                    TableName = index.TableName
                });
                if (!deleteIndex.Succeeded)
                {
                    return result.Failed(deleteIndex.Errors.ToArray());
                }
            }

            // Next attempt to drop the catalog
            var deleteCatalog = await _fullTextCatalogCommand.DeleteAsync(new SchemaFullTextCatalog()
            {
                Name = GetCatalogName()
            });

            if (!deleteCatalog.Succeeded)
            {
                return result.Failed(deleteCatalog.Errors.ToArray());
            }

            return result.Success();

        }

        public async Task<ICommandResultBase> RebuildCatalogAsync()
        {
            var result = new CommandResultBase();

            // Next attempt to drop the catalog
            var rebuildCatalog = await _fullTextCatalogCommand.UpdateAsync(new SchemaFullTextCatalog()
            {
                Name = GetCatalogName()
            });

            if (!rebuildCatalog.Succeeded)
            {
                return result.Failed(rebuildCatalog.Errors.ToArray());
            }

            return result.Success();
            
        }
    
        // -----------

        string GetCatalogName()
        {
            var catalogName = _shellSettings.Location;
            if (string.IsNullOrEmpty(catalogName))
            {
                throw new ArgumentNullException(nameof(catalogName));
            }
            return catalogName;
        }

        string GetTableName(string tableName)
        {
            return !string.IsNullOrEmpty(_shellSettings.TablePrefix)
                ? _shellSettings.TablePrefix + tableName
                : tableName;
        }

    }

}
