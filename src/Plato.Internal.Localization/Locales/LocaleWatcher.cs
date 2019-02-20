using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Locales
{
    
    public class LocaleWatcher : ILocaleWatcher
    {

        private static bool _watching = false;

        private readonly ILocaleProvider _localeProvider;
        private readonly ILocaleStore _localeStore;
        private readonly ILogger<LocaleWatcher> _logger;
        private readonly IOptions<LocaleOptions> _localeOptions;

        public LocaleWatcher(
            ILocaleProvider localeProvider,
            ILocaleStore localeStore,
            ILogger<LocaleWatcher> logger,
            IOptions<LocaleOptions> localeOptions)
        {
            _localeProvider = localeProvider;
            _localeStore = localeStore;
            _logger = logger;
            _localeOptions = localeOptions;
        }

        public async Task WatchForChanges()
        {

            // Is watching enabled?
            if (!_localeOptions.Value.WatchForChanges)
            {
                return;
            }

            if (_watching == false)
            {
                var locales = await _localeProvider.GetLocalesAsync();
                if (locales != null)
                {
                    foreach (var locale in locales)
                    {

                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Attempting to watch for changes to locale directory at '{0}'.", locale.Descriptor.DirectoryInfo.FullName);
                        }

                        var watcher = new FileSystemWatcher
                        {
                            Path = @locale.Descriptor.DirectoryInfo.FullName
                        };

                        watcher.Changed += async (sender, args) =>
                        {
                            _localeProvider.Dispose();
                            await _localeStore.DisposeAsync();
                        };
                      
                        watcher.EnableRaisingEvents = true;

                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Successfully watching for changes to locale directory at '{0}'.", watcher.Path);
                        }

                    }

                }
                _watching = true;
            }

        }

    }

}
