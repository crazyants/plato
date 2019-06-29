using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Twitter.Models;
using Plato.Twitter.Stores;

namespace Plato.Twitter.Configuration
{
    public class TwitterOptionsConfiguration : IConfigureOptions<TwitterOptions>
    {

        private readonly ITwitterSettingsStore<TwitterSettings> _TwitterSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<TwitterOptionsConfiguration> _logger;

        public TwitterOptionsConfiguration(
            ITwitterSettingsStore<TwitterSettings> TwitterSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<TwitterOptionsConfiguration> logger)
        {
            _TwitterSettingsStore = TwitterSettingsStore;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public void Configure(TwitterOptions options)
        {

            var settings = _TwitterSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {

                // ------------------
                // Consumer Keys
                // ------------------

                options.ConsumerKey = settings.ConsumerKey;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.ConsumerSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(TwitterOptionsConfiguration));
                        options.ConsumerSecret = protector.Unprotect(settings.ConsumerSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem decrypting the twitter consumer key secret. {e.Message}");
                    }
                }

                // ------------------
                // Access Tokens
                // ------------------

                options.AccessToken = settings.AccessToken;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.AccessTokenSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(TwitterOptionsConfiguration));
                        options.AccessTokenSecret = protector.Unprotect(settings.AccessTokenSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem decrypting the twitter access token secret. {e.Message}");
                    }
                }
                
            }

        }

    }
}
