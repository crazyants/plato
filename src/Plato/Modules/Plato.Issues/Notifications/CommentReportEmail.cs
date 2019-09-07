using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Issues.NotificationTypes;
using Plato.Entities;
using Plato.Entities.Models;
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

namespace Plato.Issues.Notifications
{

    public class CommentReportEmail : INotificationProvider<ReportSubmission<Comment>>
    {

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IEntityStore<Issue> _articleStore;
        private readonly IContextFacade _contextFacade;
        private readonly IEmailManager _emailManager;
        private readonly ILocaleStore _localeStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public CommentReportEmail(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IEntityStore<Issue> articleStore,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _contextFacade = contextFacade;
            _articleStore = articleStore;
            _emailManager = emailManager;
            _localeStore = localeStore;
            
            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<ReportSubmission<Comment>>> SendAsync(INotificationContext<ReportSubmission<Comment>> context)
        {

            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.IssueCommentReport.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<ReportSubmission<Comment>>();

            // Get email template
            const string templateId = "NewIssueCommentReport";

            // Tasks run in a background thread and don't have access to HttpContext
            // Create a dummy principal to represent the user so we can still obtain
            // the current culture for the email
            var principal = await _claimsPrincipalFactory.CreateAsync((User) context.Notification.To);
            var culture = await _contextFacade.GetCurrentCultureAsync(principal.Identity);
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email == null)
            {
                return result.Failed(
                    $"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");
            }

            // Get entity for reply
            var entity = await _articleStore.GetByIdAsync(context.Model.What.EntityId);

            // We need an entity for the reply
            if (entity == null)
            {
                return result.Failed(
                    $"No entity with id '{context.Model.What.EntityId}' exists. Failed to send reply spam email notification.");
            }

            // Build entity url
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
            var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Issues",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = context.Model.What.Id
            });

            // Reason given text
            var reasonText = S["None Provided"];
            if (ReportReasons.Reasons.ContainsKey(context.Model.Why))
            {
                reasonText = S[ReportReasons.Reasons[context.Model.Why]];
            }
            
            // Build message from template
            var message = email.BuildMailMessage();
            message.Body = string.Format(
                email.Message,
                context.Notification.To.DisplayName,
                entity.Title,
                reasonText.Value,
                context.Model.Who.DisplayName,
                context.Model.Who.UserName,
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

    }
}
