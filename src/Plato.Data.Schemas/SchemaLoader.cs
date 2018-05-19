using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Abstractions.Extensions;
using Plato.FileSystem.Abstractions;
using Plato.Shell.Abstractions;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Schemas
{
    public class SchemaLoader : ISchemaLoader
    {
        private const string InstallDirectory = "install";
        private const string UpgradeDirectory = "upgrade";
        private const string RollbackDirectory = "rollback";
        private const string SchemaExtension = ".sql";

        private static readonly ConcurrentDictionary<string, SchemaDescriptor> _loadedSchemas =
            new ConcurrentDictionary<string, SchemaDescriptor>(StringComparer.OrdinalIgnoreCase);

        private List<string> _versions;
        
        private readonly IAppDataFolder _appDataFolder;
        private readonly IOptions<ShellOptions> _optionsAccessor;
        private readonly ILogger<SchemaLoader> _logger;

        public SchemaLoader(
            IAppDataFolder appDataFolder,
            IOptions<ShellOptions> optionsAccessor,
            ILogger<SchemaLoader> logger)
        {
            _appDataFolder = appDataFolder;
            _optionsAccessor = optionsAccessor;
            _logger = logger;
        }

        public List<SchemaDescriptor> LoadedSchemas => (List<SchemaDescriptor>) _loadedSchemas.Values;

        public void LoadSchemas()
        {
            LoadSchemas(new List<string>());
        }

        public void LoadSchemas(List<string> versions)
        {
            _versions = versions;
            if (_loadedSchemas.Count == 0)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Loading schemas from '{0}'.", _optionsAccessor.Value.SchemaLocation);
                LoadSchemasInternal();
            }
        }

        #region "Private Methods"

        private void LoadSchemasInternal()
        {
            foreach (var schemaFolder in _appDataFolder.ListDirectories(_optionsAccessor.Value.SchemaLocation))
            {
                var specified = IsSchemaSpecified(schemaFolder.Name);
                var notLoaded = !IsSchemaLoaded(schemaFolder.Name);
                if (specified && notLoaded)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("Schema folder found '{0}'.", schemaFolder.Name);
                    _loadedSchemas.TryAdd(schemaFolder.Name, LoadSchema(schemaFolder));
                }
            }
        }

        private SchemaDescriptor LoadSchema(DirectoryInfo schemaFolder)
        {

            var installSql = "";
            var upgradeSql = "";
            var rollbackSql = "";

            /* Each schema folder contains 3 child folders.
             
                - SchemaFolder
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
                        switch (folder.Name.ToLower())
                        {
                            case InstallDirectory:
                            {

                                installSql = ReadFile(file.FullName);
                                break;
                            }
                            case UpgradeDirectory:
                            {
                                upgradeSql = ReadFile(file.FullName);
                                    break;
                            }
                            case RollbackDirectory:
                            {
                                rollbackSql = ReadFile(file.FullName);
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

            return new SchemaDescriptor()
            {
                Version = schemaFolder.Name,
                InstallSql = installSql,
                UpgradeSql = upgradeSql,
                RollbackSql = rollbackSql
            };

        }

        private string ReadFile(string path)
        {
            var fs = new FileStream(path, FileMode.Open);
            using (var reader = new StreamReader(fs))
            {
                return reader.ReadToEnd();
            }
        }

        private bool IsSchemaLoaded(string version)
        {
            return _loadedSchemas.ContainsKey(version);
        }

        private bool IsSchemaSpecified(string version)
        {
            if (_versions.Count == 0)
            {
                return false;
            }
            return _versions.Contains(version);
            
        }

        #endregion


    }
}
