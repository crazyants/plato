using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Email.Models;
using Plato.Internal.Abstractions;

namespace Plato.Email.Services
{

    public class SmtpService : ISmtpService
    {

        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<SmtpService> _logger;
        
        public SmtpService(
            SmtpSettings smtpSettings, 
            ILogger<SmtpService> logger)
        {
            _smtpSettings = smtpSettings;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<IActivityResult<MailMessage>> SendAsync(MailMessage message)
        {

            var result = new SmtpResult();
            try
            {
                using (var client = GetClient())
                {
                    await client.SendMailAsync(message);
                    
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {

                        _logger.LogDebug("Email sent successfully. From: {0}, To: {1}, Subject{ 2}, Date: {3}'",
                            message.From.Address,
                            String.Join(",", message.To.Select(t => t.Address).ToArray()),
                            message.Subject,
                            DateTime.UtcNow);
                    }

                    return result.Success(message);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "A crtical exception occurred whilst sending an email message. From: {0}, To: {1}, Subject{ 2}, Date: {3}'",
                    message.From.Address,
                    String.Join(",", message.To.Select(t => t.Address).ToArray()),
                    message.Subject,
                    DateTime.UtcNow);
                return result.Failed($"An error occurred while sending an email: '{e.Message}'");
            }

        }

        #endregion

        #region "Private Methods"

        private SmtpClient GetClient()
        {
            var smtp = new SmtpClient()
            {
                DeliveryMethod = _smtpSettings.DeliveryMethod
            };

            switch (smtp.DeliveryMethod)
            {

                case SmtpDeliveryMethod.Network:

                    smtp.Host = _smtpSettings.Host;
                    smtp.Port = _smtpSettings.Port;
                    smtp.EnableSsl = _smtpSettings.EnableSsl;

                    smtp.UseDefaultCredentials = _smtpSettings.RequireCredentials && _smtpSettings.UseDefaultCredentials;

                    if (_smtpSettings.RequireCredentials)
                    {
                        if (_smtpSettings.UseDefaultCredentials)
                        {
                            smtp.UseDefaultCredentials = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(_smtpSettings.UserName))
                        {
                            smtp.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password);
                        }
                    }

                    break;

                case SmtpDeliveryMethod.PickupDirectoryFromIis:

                    // Nothing to configure
                    break;

                case SmtpDeliveryMethod.SpecifiedPickupDirectory:

                    smtp.PickupDirectoryLocation = _smtpSettings.PickupDirectoryLocation;
                    break;

                default:

                    throw new NotSupportedException($"The '{smtp.DeliveryMethod}' delivery method is not supported."); ;
            }

            return smtp;
        }

        #endregion
        
    }

}

