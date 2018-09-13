using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Models;
using Plato.Email.Stores;

namespace Plato.Email.Configuration
{
    public class SmtpSettingsConfiguration : IConfigureOptions<SmtpSettings>
    {
        
        private readonly IEmailSettingsStore<EmailSettings> _emailSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<SmtpSettingsConfiguration> _logger;

        public SmtpSettingsConfiguration(
            IEmailSettingsStore<EmailSettings> emailSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<SmtpSettingsConfiguration> logger)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _emailSettingsStore = emailSettingsStore;
            _logger = logger;
        }

        public void Configure(SmtpSettings options)
        {

            var settings = _emailSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            // We have no settings to configure
            if (settings == null)
            {
                return;
            }

            var smtpSettings = settings.SmtpSettings;
            if (smtpSettings != null)
            {

                options.DefaultFrom = smtpSettings.DefaultFrom;
                options.DeliveryMethod = smtpSettings.DeliveryMethod;
                options.PickupDirectoryLocation = smtpSettings.PickupDirectoryLocation;
                options.Host = smtpSettings.Host;
                options.Port = smtpSettings.Port;
                options.EnableSsl = smtpSettings.EnableSsl;
                options.RequireCredentials = smtpSettings.RequireCredentials;
                options.UseDefaultCredentials = smtpSettings.UseDefaultCredentials;
                options.UserName = smtpSettings.UserName;

                // Decrypt the password
                if (!String.IsNullOrWhiteSpace(smtpSettings.Password))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(SmtpSettingsConfiguration));
                        options.Password = protector.Unprotect(smtpSettings.Password);
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
