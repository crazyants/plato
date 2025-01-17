﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Plato.Internal.Yaml;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Yaml.Extensions;

namespace Plato.Internal.Shell
{
    public class ShellSettingsManager : IShellSettingsManager
    {

        #region "Private Variables"

        private readonly IOptions<ShellOptions> _optionsAccessor;
        private readonly IAppDataFolder _appDataFolder;        
        private readonly ILogger _logger;

        private const string SettingsFileNameFormat = "Settings.{0}";

        #endregion

        #region "Constrcutor"
        
        public ShellSettingsManager(
            IAppDataFolder appDataFolder,        
            IOptions<ShellOptions> optionsAccessor,
            ILogger<ShellSettingsManager> logger)
        {
            _optionsAccessor = optionsAccessor;
            _appDataFolder = appDataFolder;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        IEnumerable<ShellSettings> IShellSettingsManager.LoadSettings()
        {

            var shellSettings = new List<ShellSettings>();
            foreach (var tenant in _appDataFolder.ListDirectories(_optionsAccessor.Value.Location))
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("ShellSettings found in '{0}', attempting to load.", tenant.Name);
                }
                var configurationContainer =
                    new ConfigurationBuilder()
                        .SetBasePath(_appDataFolder.RootPath)
                        .AddJsonFile(_appDataFolder.Combine(tenant.FullName, string.Format(SettingsFileNameFormat, "json")),
                            true)
                        .AddXmlFile(_appDataFolder.Combine(tenant.FullName, string.Format(SettingsFileNameFormat, "xml")),
                            true)
                        .AddYamlFile(_appDataFolder.Combine(tenant.FullName, string.Format(SettingsFileNameFormat, "txt")),
                            false);

                var config = configurationContainer.Build();
                var shellSetting = ShellSettingsSerializer.ParseSettings(config);
                shellSetting.Location = tenant.Name;
                shellSettings.Add(shellSetting);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Loaded ShellSettings for tenant '{0}'", shellSetting.Name);
                }
            }

            return shellSettings;
        }

        void IShellSettingsManager.SaveSettings(IShellSettings shellSettings)
        {

            if (shellSettings == null)
            {
                throw new ArgumentNullException(nameof(shellSettings));
            }

            if (string.IsNullOrEmpty(shellSettings.Name))
            {
                throw new ArgumentNullException(nameof(shellSettings.Name));
            }
                
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Saving ShellSettings for tenant '{0}'", shellSettings.Name);
            }

            var fileName = string.Format(SettingsFileNameFormat, "txt");
            var tenantPath = _appDataFolder.MapPath(
                _appDataFolder.Combine(
                    _optionsAccessor.Value.Location,
                    shellSettings.Location,
                    fileName));

            var configurationProvider = new YamlConfigurationProvider(new YamlConfigurationSource
            {
                Path = tenantPath,
                Optional = false
            });

            foreach (var key in shellSettings.Keys)
            {
                if (!string.IsNullOrEmpty(shellSettings[key]))
                {
                    configurationProvider.Set(key, shellSettings[key]);
                }
            }

            configurationProvider.Commit();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Saved ShellSettings for tenant '{0}'", shellSettings.Name);
            }
        }

        #endregion

    }

}
