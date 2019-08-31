using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Slack.Models;
using Plato.Slack.Stores;

namespace Plato.Slack.Configuration
{
    public class SlackOptionsConfiguration : IConfigureOptions<SlackOptions>
    {

        private readonly ISlackSettingsStore<SlackSettings> _slackSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<SlackOptionsConfiguration> _logger;

        public SlackOptionsConfiguration(
            ISlackSettingsStore<SlackSettings> slackSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<SlackOptionsConfiguration> logger)
        {
            _slackSettingsStore = slackSettingsStore;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public void Configure(SlackOptions options)
        {

            var settings = _slackSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                
                if (!String.IsNullOrWhiteSpace(settings.WebHookUrl))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(SlackOptionsConfiguration));
                        options.WebHookUrl = protector.Unprotect(settings.WebHookUrl);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem decrypting the Slack Web Hook URL - {e.Message}");
                    }
                }

            }

        }

    }
}
