using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Abstractions.Extensions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Shell.Abstractions;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Internal.Data.Schemas
{
    public class SchemaLoader : ISchemaLoader
    {
        private const string InstallDirectory = "install";
        private const string UpgradeDirectory = "upgrade";
        private const string RollbackDirectory = "rollback";
        private const string SchemaExtension = ".sql";

        private static readonly ConcurrentDictionary<string, Schema> _loadedSchemas =
            new ConcurrentDictionary<string, Schema>(StringComparer.OrdinalIgnoreCase);

        private List<string> _versions;
        
        private readonly IAppDataFolder _appDataFolder;
        private readonly IOptions<ShellOptions> _optionsAccessor;
        private readonly ILogger<SchemaLoader> _logger;
        
        public List<Schema> Schemas => (List<Schema>)_loadedSchemas.Values.ToList() ?? null;
        
        public SchemaLoader(
            IAppDataFolder appDataFolder,
            IOptions<ShellOptions> optionsAccessor,
            ILogger<SchemaLoader> logger)
        {
            _appDataFolder = appDataFolder;
            _optionsAccessor = optionsAccessor;
            _logger = logger;
        }

        public async Task<ISchemaLoader> LoadAsync(List<string> versions)
        {
            _versions = versions;
            if (_loadedSchemas?.Count == 0)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Loading schemas from '{0}'.", _optionsAccessor.Value.SchemaLocation);
                await LoadSchemasInternal();
            }
            return await Task.FromResult(this);
        }

        #region "Private Methods"

        async Task LoadSchemasInternal()
        {
            foreach (var schemaFolder in _appDataFolder.ListDirectories(_optionsAccessor.Value.SchemaLocation))
            {
                var specified = IsSchemaSpecified(schemaFolder.Name);
                var notLoaded = !IsSchemaLoaded(schemaFolder.Name);
                if (specified && notLoaded)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("Schema folder found '{0}'.", schemaFolder.Name);

                    var schema = await LoadSchema(schemaFolder);
                    _loadedSchemas.TryAdd(schemaFolder.Name, schema);
                }
            }
        }

        async Task<Schema> LoadSchema(DirectoryInfo schemaFolder)
        {

            string installSql = null,
                upgradeSql = null,
                rollbackSql = null;

            /* Each schema folder contains 3 child folders.
             
                - SchemaFolder - 1.0.0, 2.1.4 etc
                    - install
                    - upgrade
                    - rollback

             */

            var subFolders = schemaFolder.GetDirectories();
            foreach (var folder in subFolders)
            {
                foreach (var file in folder.GetFiles())
                {
                    if (file.Extension.ToLower() == SchemaExtension)
                    {
                        var path = _appDataFolder.Combine(
                            _optionsAccessor.Value.SchemaLocation,
                            schemaFolder.Name,
                            folder.Name,
                            file.Name);
                        switch (folder.Name.ToLower())
                        {
                            case InstallDirectory:
                            {
                                installSql = await ReadFileAsync(path);
                                break;
                            }
                            case UpgradeDirectory:
                            {
                                upgradeSql = await ReadFileAsync(path);
                                break;
                            }
                            case RollbackDirectory:
                            {
                                rollbackSql = await ReadFileAsync(path);
                                break;
                            }
                        }
                    }
                }
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Schema loaded successfully '{0}'.", schemaFolder.Name);
            }

            return new Schema()
            {
                Version = schemaFolder.Name,
                InstallSql = installSql,
                UpgradeSql = upgradeSql,
                RollbackSql = rollbackSql
            };
            
        }

        async Task<string> ReadFileAsync(string path)
        {
            return await _appDataFolder.ReadFileAsync(path);
        }

        bool IsSchemaLoaded(string version)
        {
            return _loadedSchemas.ContainsKey(version);
        }

        bool IsSchemaSpecified(string version)
        {
            // no explict versions - allow all schemas
            if (_versions.Count == 0)
                return true;
            // check if version if specified 
            return _versions.Contains(version);
            
        }

        #endregion


    }
}
