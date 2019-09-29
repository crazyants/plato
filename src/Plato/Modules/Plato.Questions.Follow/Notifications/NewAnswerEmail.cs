using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Plato.Questions.Follow.NotificationTypes;
using Plato.Questions.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;


namespace Plato.Questions.Follow.Notifications
{

    public class NewAnswerEmail : INotificationProvider<Answer>
    {

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IEntityStore<Question> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IEmailManager _emailManager;
        private readonly ILocaleStore _localeStore;

        public NewAnswerEmail(
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IEntityStore<Question> entityStore,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
            _entityStore = entityStore;
        }

        public async Task<ICommandResult<Answer>> SendAsync(INotificationContext<Answer> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.NewAnswer.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Answer>();
            
            // Get the entity for the reply
            var entity = await _entityStore.GetByIdAsync(context.Model.EntityId);
            if (entity == null)
            {
                return result.Failed($"No entity could be found with the Id of {context.Model.EntityId} when sending the follow notification '{EmailNotifications.NewAnswer.Name}'.");
            }

            // Get email template
            const string templateId = "NewQuestionAnswer";

            // Tasks run in a background thread and don't have access to HttpContext
            // Create a dummy principal to represent the user so we can still obtain
            // the current culture for the email
            var principal = await _claimsPrincipalFactory.CreateAsync((User)context.Notification.To);
            var culture = await _contextFacade.GetCurrentCultureAsync(principal.Identity);
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email != null)
            {

                // Build entity url
                var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
                var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
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
