using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Models;
using Plato.Email.Stores;

namespace Plato.Email.Services
{
    public class SmtpSettingsConfiguration : IConfigureOptions<SmtpSettings>
    {
        
        private IEmailSettingsStore<EmailSettings> _emailSettingsStore;
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

            var settings = _emailSettingsStore.GetAsync()
                .GetAwaiter()
                .GetResult()
                .SmtpSettings;

            // Set options
            options.DefaultFrom = settings.DefaultFrom;
            options.DeliveryMethod = settings.DeliveryMethod;
            options.PickupDirectoryLocation = settings.PickupDirectoryLocation;
            options.Host = settings.Host;
            options.Port = settings.Port;
            options.EnableSsl = settings.EnableSsl;
            options.RequireCredentials = settings.RequireCredentials;
            options.UseDefaultCredentials = settings.UseDefaultCredentials;
            options.UserName = settings.UserName;

            // Decrypt the password
            if (!String.IsNullOrWhiteSpace(settings.Password))
            {
                try
                {
                    var protector = _dataProtectionProvider.CreateProtector(nameof(SmtpSettingsConfiguration));
                    options.Password = protector.Unprotect(settings.Password);
                }
                catch
                {
                    _logger.LogError("There was a problem decrypting the SMTP password.");
                }
            }

        }

    }

}
