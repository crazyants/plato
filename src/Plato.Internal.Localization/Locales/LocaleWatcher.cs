using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Localization.Abstractions;

namespace Plato.Internal.Localization.Locales
{
    
    public class LocaleWatcher : ILocaleWatcher
    {

        private static bool _watching = false;

        private readonly ILocaleProvider _localeProvider;
        private readonly ILocaleStore _localeStore;
        private readonly ILogger<LocaleWatcher> _logger;

        public LocaleWatcher(
            ILocaleProvider localeProvider,
            ILocaleStore localeStore,
            ILogger<LocaleWatcher> logger)
        {
            _localeProvider = localeProvider;
            _localeStore = localeStore;
            _logger = logger;
        }

        public async Task WatchForChanges()
        {

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

                        watcher.Changed += (sender, args) =>
                        {
                            _localeProvider.Dispose();
                            _localeStore.Dispose();
                        };
                      
                        watcher.EnableRaisingEvents = true;

                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Successfully watchjing for changes to locale directory at '{0}'.", watcher.Path);
                        }

                    }

                }
                _watching = true;
            }

        }

    }

}
