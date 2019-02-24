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
using Plato.Search.Stores;

namespace Plato.Search.Services
{
       
    public class FullTextCatalogManager : IFullTextCatalogManager
    {

        private readonly IConstraintStore _constraintStore;
        private readonly IFullTextCatalogCommand<SchemaFullTextCatalog> _fullTextCatalogCommand;
        private readonly IFullTextIndexCommand<SchemaFullTextIndex> _fullTextIndexCommand;
        private readonly IFullTextIndexManager _fullTextIndexManager;
        private readonly IFullTextIndexStore _fullTextIndexStore;
        private readonly IShellSettings _shellSettings;

        public FullTextCatalogManager(
            IFullTextCatalogCommand<SchemaFullTextCatalog> fullTextCatalogCommand,
            IFullTextIndexCommand<SchemaFullTextIndex> fullTextIndexCommand, 
            IFullTextIndexManager fullTextIndexManager,
            IShellSettings shellSettings,
            IFullTextIndexStore fullTextIndexStore,
            IConstraintStore constraintStore)
        {
            _fullTextCatalogCommand = fullTextCatalogCommand;
            _fullTextIndexCommand = fullTextIndexCommand;
            _fullTextIndexManager = fullTextIndexManager;
            _shellSettings = shellSettings;
            _fullTextIndexStore = fullTextIndexStore;
            _constraintStore = constraintStore;
        }

        public async Task<ICommandResultBase> CreateCatalogAsync()
        {

            // Build result
            var result = new CommandResultBase();
            
            // Create catalog
            var createCatalog = await CreateCatalogInternalAsync();
            if (!createCatalog.Succeeded)
            {
                return result.Failed(createCatalog.Errors.ToArray());
            }
            
            // Create indexes
            var createIndexes = await CreateIndexesInternalAsync(GetCatalogName());
            if (!createIndexes.Succeeded)
            {
                return result.Failed(createIndexes.Errors.ToArray());
            }

            return result.Success();

        }

        public async Task<ICommandResultBase> DropCatalogAsync()
        {

            // Build result
            var result = new CommandResultBase();

            // Drop indexes
            var dropIndexes = await DropIndexesInternalAsync();
            if (!dropIndexes.Succeeded)
            {
                return result.Failed(dropIndexes.Errors.ToArray());
            }

            // Drop catalog
            var dropCatalog = await DropCatalogInternalAsync();
            if (!dropCatalog.Succeeded)
            {
                return result.Failed(dropCatalog.Errors.ToArray());
            }

            return result.Success();

        }

        public async Task<ICommandResultBase> RebuildCatalogAsync()
        {
            
            // Build result
            var result = new CommandResultBase();
            
            // Create indexes
            var createIndexes = await CreateIndexesInternalAsync(GetCatalogName());
            if (!createIndexes.Succeeded)
            {
                return result.Failed(createIndexes.Errors.ToArray());
            }

            // Rebuild the catalog
            var rebuildCatalog = await RebuildCatalogInternalAsync();
            if (!rebuildCatalog.Succeeded)
            {
                return result.Failed(rebuildCatalog.Errors.ToArray());
            }

            return result.Success();
            
        }

        // -----------

        async Task<ICommandResultBase> CreateCatalogInternalAsync()
        {

            // Build result
            var result = new CommandResultBase();

            // Create catalog
            var createCatalog = await _fullTextCatalogCommand.CreateAsync(new SchemaFullTextCatalog()
            {
                Name = GetCatalogName()
            });
            if (!createCatalog.Succeeded)
            {
                return result.Failed(createCatalog.Errors.ToArray());
            }

            return result.Success();

        }

        async Task<ICommandResultBase> DropCatalogInternalAsync()
        {

            // Build result
            var result = new CommandResultBase();

            // Drop catalog
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

        async Task<ICommandResultBase> RebuildCatalogInternalAsync()
        {

            // Build result
            var result = new CommandResultBase();
        
            // Rebuild the catalog
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
        
        async Task<ICommandResultBase> CreateIndexesInternalAsync(string catalogName)
        {

            // Build result
            var result = new CommandResultBase();

            // Get installed indexes from database
            var installedIndexes = await _fullTextIndexStore.SelectIndexesAsync();
            
            // Get all provided indexes
            var providedIndexes = _fullTextIndexManager.GetIndexes();

            // Interate provided indexes
            foreach (var providedIndex in providedIndexes)
            {

                // Attempt to get existing index within database
                var existingIndex = installedIndexes?.FirstOrDefault(i =>
                    i.TableName == providedIndex.TableName && providedIndex.ColumnNames.Contains(i.ColumnName));

                // Index does not already exist within the database
                if (existingIndex == null)
                {

                    // Get table name with prefix
                    var tableName = GetTableName(providedIndex.TableName);

                    // Get primary key constraint for table
                    var primaryKey = await _constraintStore.GetPrimaryKeyConstraint(tableName);
                    if (primaryKey == null)
                    {
                        return result.Failed($"Could not find a primary key constraint for table {tableName}");
                    }
               
                    // Create the index
                    var createIndex = await _fullTextIndexCommand.CreateAsync(new SchemaFullTextIndex()
                    {
                        PrimaryKeyName = primaryKey.ConstraintName,
                        TableName = providedIndex.TableName,
                        ColumnNames = providedIndex.ColumnNames,
                        LanguageCode = providedIndex.LanguageCode,
                        FillFactor = providedIndex.FillFactor,
                        CatalogName = catalogName
                    });

                    if (!createIndex.Succeeded)
                    {
                        return result.Failed(createIndex.Errors.ToArray());
                    }

                }
              
            }

            return result.Success();

        }

        async Task<ICommandResultBase> DropIndexesInternalAsync()
        {

            // Build result
            var result = new CommandResultBase();

            // Get provided indexes
            var providedIndexes = _fullTextIndexManager.GetIndexes();

            // Drop all provided indexes
            foreach (var providedIndex in providedIndexes)
            {
                var deleteIndex = await _fullTextIndexCommand.DeleteAsync(new SchemaFullTextIndex()
                {
                    TableName = providedIndex.TableName
                });
                if (!deleteIndex.Succeeded)
                {
                    return result.Failed(deleteIndex.Errors.ToArray());
                }
            }

            return result.Success();

        }

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
