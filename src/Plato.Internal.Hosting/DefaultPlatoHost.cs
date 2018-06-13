using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Plato.Internal.Shell;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Shell.Abstractions.Models;

namespace Plato.Internal.Hosting
{
    public class DefaultPlatoHost : IPlatoHost
    {

        #region "Private Variables"

        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly IRunningShellTable _runningShellTable;
        private readonly ILogger _logger;
   
        private static readonly object _syncLock = new object();
        private ConcurrentDictionary<string, ShellContext> _shellContexts;

        #endregion

        #region "Constructor"

        public DefaultPlatoHost(
            IShellSettingsManager shellSettingsManager,
            IShellContextFactory shellContextFactory,
            IRunningShellTable runningShellTable, 
            ILogger<DefaultPlatoHost> logger)
        {
            _shellSettingsManager = shellSettingsManager;
            _shellContextFactory = shellContextFactory;
            _runningShellTable = runningShellTable;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public void Initialize()
        {
            BuildCurrent();
        }
  
        public ShellContext GetOrCreateShellContext(ShellSettings settings)
        {
            if (_shellContexts == null)
                _shellContexts = new ConcurrentDictionary<string, ShellContext>();
            return _shellContexts.GetOrAdd(settings.Name, tenant =>
            {
                var shellContext = CreateShellContext(settings);
                ActivateShell(shellContext);
                return shellContext;
            });
        }

        public void UpdateShellSettings(ShellSettings settings)
        {
            _shellSettingsManager.SaveSettings(settings);
            _runningShellTable.Remove(settings);
            if (_shellContexts.TryRemove(settings.Name, out var context))
            {
                if (_shellContexts.Count == 0)
                    _shellContexts = null;
                context.Dispose();
            }
            GetOrCreateShellContext(settings);
        }
        
        public ShellContext CreateShellContext(ShellSettings settings)
        {
            if (settings.State == TenantState.Uninitialized)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Creating shell context for tenant {0} setup", settings.Name);
                }
                return _shellContextFactory.CreateSetupContext(settings);
            }

            _logger.LogDebug("Creating shell context for tenant {0}", settings.Name);
            return _shellContextFactory.CreateShellContext(settings);
        }
        
        #endregion

        #region "Private Methods"

        IDictionary<string, ShellContext> BuildCurrent()
        {

            if (_shellContexts == null)
            {
                lock (_syncLock)
                {
                    if (_shellContexts == null)
                    {
                        _shellContexts = new ConcurrentDictionary<string, ShellContext>();
                        CreateAndActivateShells();
                    }
                }
            }

            return _shellContexts;
        }
        
        void CreateAndActivateShells()
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Start creation of shells");
            
            // Is there any tenant right now?
            var allSettings = _shellSettingsManager.LoadSettings().Where(CanCreateShell).ToArray();
            
            // Load all tenants, and activate their shell.
            if (allSettings.Any())
            {
                //Parallel.ForEach(allSettings, settings =>
                //{
                foreach (var settings in allSettings)
                {
                    GetOrCreateShellContext(settings);
                }
                //});
            } 
            else
            {
                // No settings, run the Setup.
                var setupContext = CreateSetupContext();
                ActivateShell(setupContext);
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Done creating shells");
            }
        }


        void ActivateShell(ShellContext context)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Activating context for tenant {0}", context.Settings.Name);
            
            if (_shellContexts.TryAdd(context.Settings.Name, context))
                _runningShellTable.Add(context.Settings);
        }
        
        ShellContext CreateSetupContext()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Creating shell context for root setup.");
            return _shellContextFactory.CreateSetupContext(ShellHelper.BuildDefaultUninitializedShell);
        }

        /// <summary>
        /// Whether or not a shell can be added to the list of available shells.
        /// </summary>
        private bool CanCreateShell(ShellSettings shellSettings)
        {
            return
                shellSettings.State == TenantState.Running ||
                shellSettings.State == TenantState.Uninitialized ||
                shellSettings.State == TenantState.Initializing ||
                shellSettings.State == TenantState.Disabled;
        }
        
        #endregion
        
    }
}
