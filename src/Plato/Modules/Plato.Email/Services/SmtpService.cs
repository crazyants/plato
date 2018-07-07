using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Email.Models;
using Plato.Internal.Abstractions;

namespace Plato.Email.Services
{

    public interface ISmtpService
    {
        Task<IActivityResult<MailMessage>> SendAsync(MailMessage message);

    }

    public class SmtpService : ISmtpService
    {

        private readonly SmtpSettings _options;
        private readonly ILogger<SmtpService> _logger;
        
        public SmtpService(SmtpSettings options, ILogger<SmtpService> logger)
        {
            _options = options;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<IActivityResult<MailMessage>> SendAsync(MailMessage message)
        {

            var result = new SmtpResult();

            if (_options?.DefaultFrom == null)
            {
                return result.Failed("SMTP settings must be configured before an email can be sent.");
            }

            if (message.From == null)
            {
                message.From = new MailAddress(_options.DefaultFrom);
            }

            try
            {
                using (var client = GetClient())
                {
                    await client.SendMailAsync(message);
                    return result.Success(message);
                }
            }
            catch (Exception e)
            {
                return result.Failed($"An error occurred while sending an email: '{e.Message}'");
            }

        }

        #endregion

        #region "Private Methods"

        private SmtpClient GetClient()
        {
            var smtp = new SmtpClient()
            {
                DeliveryMethod = _options.DeliveryMethod
            };

            switch (smtp.DeliveryMethod)
            {

                case SmtpDeliveryMethod.Network:

                    smtp.Host = _options.Host;
                    smtp.Port = _options.Port;
                    smtp.EnableSsl = _options.EnableSsl;

                    smtp.UseDefaultCredentials = _options.RequireCredentials && _options.UseDefaultCredentials;

                    if (_options.RequireCredentials)
                    {
                        if (_options.UseDefaultCredentials)
                        {
                            smtp.UseDefaultCredentials = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(_options.UserName))
                        {
                            smtp.Credentials = new NetworkCredential(_options.UserName, _options.Password);
                        }
                    }

                    break;

                case SmtpDeliveryMethod.PickupDirectoryFromIis:

                    // Nothing to configure
                    break;

                case SmtpDeliveryMethod.SpecifiedPickupDirectory:

                    smtp.PickupDirectoryLocation = _options.PickupDirectoryLocation;
                    break;

                default:

                    throw new NotSupportedException($"The '{smtp.DeliveryMethod}' delivery method is not supported."); ;
            }

            return smtp;
        }

        #endregion
        
    }

}

