using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Facebook.Models;
using Plato.Facebook.Stores;

namespace Plato.Facebook.Configuration
{
    public class FacebookOptionsConfiguration : IConfigureOptions<FacebookOptions>
    {

        private readonly IFacebookSettingsStore<FacebookSettings> _facebookSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<FacebookOptionsConfiguration> _logger;

        public FacebookOptionsConfiguration(
            IFacebookSettingsStore<FacebookSettings> facebookSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<FacebookOptionsConfiguration> logger)
        {
            _facebookSettingsStore = facebookSettingsStore;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public void Configure(FacebookOptions options)
        {

            var settings = _facebookSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                options.AppId = settings.AppId;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.AppSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(FacebookOptionsConfiguration));
                        options.AppSecret = protector.Unprotect(settings.AppSecret);
                    }
                    catch
                    {
                        _logger.LogError("There was a problem decrypting the SMTP password.");
                    }
                }


            }
        }

    }
}
