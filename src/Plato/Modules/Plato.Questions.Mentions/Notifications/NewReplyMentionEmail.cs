using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Questions.Mentions.NotificationTypes;
using Plato.Questions.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Questions.Mentions.Notifications
{


    public class NewReplyMentionEmail : INotificationProvider<Answer>
    { 

        private readonly IEntityStore<Question> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IEmailManager _emailManager;
        private readonly ILocaleStore _localeStore;
    
        public NewReplyMentionEmail(
            IEntityStore<Question> entityStore,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
            _entityStore = entityStore;
        }

        public async Task<ICommandResult<Answer>> SendAsync(INotificationContext<Answer> context)
        {

            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.NewMention.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // We always need a model
            if (context.Model == null)
            {
                return null;
            }

            // The entity should be visible
            if (context.Model.IsHidden())
            {
                return null;
            }
            
            // Get entity for reply
            var entity = await _entityStore.GetByIdAsync(context.Model.EntityId);

            // We need an entity
            if (entity == null)
            {
                return null;
            }

            // The entity should be visible
            if (entity.IsHidden())
            {
                return null;
            }
            
            // Create result
            var result = new CommandResult<Answer>();

            // Get email template
            const string templateId = "NewQuestionsMention";
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email != null)
            {

                // Build topic url
                var baseUri = await _contextFacade.GetBaseUrlAsync();
                var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Questions",
                    ["controller"] = "Home",
                    ["action"] = "Reply",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias,
                    ["opts.replyId"] = context.Model.Id
                });

                // Build message from template
                var message = email.BuildMailMessage();
                message.Body = string.Format(
                    email.Message,
                    context.Notification.To.DisplayName,
                    entity.Title,
                    baseUri + url);
                message.IsBodyHtml = true;
                message.To.Add(new MailAddress(context.Notification.To.Email));

                // Send message
                var emailResult = await _emailManager.SaveAsync(message);
                if (emailResult.Succeeded)
                {
                    return result.Success(context.Model);
                }

                return result.Failed(emailResult.Errors?.ToArray());

            }

            return result.Failed($"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");

        }

    }

}
