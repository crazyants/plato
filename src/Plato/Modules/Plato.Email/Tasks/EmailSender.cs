using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Models;
using Plato.Email.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Emails.Abstractions.Extensions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Email.Tasks
{
    public class EmailSender : IBackgroundTaskProvider
    {

        public int IntervalInSeconds { get; private set; }


        private readonly ILogger<EmailSender> _logger;
        private readonly IEmailStore<EmailMessage> _emailStore;
        private readonly IEmailManager _emailManager;
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(
            IEmailManager emailManager,
            IOptions<SmtpSettings> smtpSettings,
            IEmailStore<EmailMessage> emailStore,
            ILogger<EmailSender> logger)
        {

            _smtpSettings = smtpSettings.Value;
            _emailManager = emailManager;
            _emailStore = emailStore;
            _logger = logger;

            // Set polling interval
            IntervalInSeconds = _smtpSettings.PollingInterval;

        }

        public async Task ExecuteAsync(object sender, SafeTimerEventArgs args)
        {

            var toDelete = new List<int>();
            var toIncrement = new List<int>();

            // 2. Get latest email batch to process
            var emails = await _emailStore.QueryAsync()
                .Take(1, _smtpSettings.BatchSize)
                .OrderBy("Id", OrderBy.Desc)
                .ToList();

            // Iterate batch attempting to send 
            foreach (var email in emails.Data)
            {

                // Attempt to send the email
                var result = await _emailManager.SendAsync(email.ToMailMessage());

                // Email sent successfully
                if (result.Succeeded)
                {
                    toDelete.Add(email.Id);
                }
                else
                {

                    // Log errors
                    if (_logger.IsEnabled(LogLevel.Critical))
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogCritical(error.Description);
                        }
                    }

                    // Increment send attempts if we are below configured threshold
                    // Once we reach the threshold finally delete the email
                    if (email.SendAttempts < _smtpSettings.SendAttempts)
                    {
                        toIncrement.Add(email.Id);
                    }
                    else
                    {
                        toDelete.Add(email.Id);
                    }

                }

            }
            
            if (toDelete.Count > 0)
            {

            }

            if (toIncrement.Count > 0)
            {

            }

        }

        Task DeleteSuccessful(IList<int> emailIds)
        {

            return Task.CompletedTask;
        }

        Task IncrementSendAttempts(IList<int> emailIds)
        {
            return Task.CompletedTask;
        }


    }

}
