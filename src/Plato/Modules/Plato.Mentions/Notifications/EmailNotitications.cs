using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Mentions.Notifications
{
    public class EmailNotifications : INotificationTypeProvider
    {
        
        public static readonly EmailNotification NewMention =
            new EmailNotification("NewMentionEmail", "New Mentions", "Show me a web notification for each new @mention.", SendNewMention());
        
        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewMention
            };

        }

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            throw new NotImplementedException();
        }

        static Action<INotificationContext> SendNewMention()
        {
            return async (context) =>
            {

                var contextFacade = context.ServiceProvider.GetRequiredService<IContextFacade>();
                var localStore = context.ServiceProvider.GetRequiredService<ILocaleStore>();
                var emailManager = context.ServiceProvider.GetRequiredService<IEmailManager>();

                // Get reset password email
                var culture = await contextFacade.GetCurrentCultureAsync();
                var email = await localStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "ConfirmEmail");
                if (email != null)
                {

                    // Build email confirmation link
                    var baseUrl = await contextFacade.GetBaseUrlAsync();
                    var callbackUrl = baseUrl + contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Users",
                        ["Controller"] = "Account",
                        ["Action"] = "ActivateAccount",
                        ["Code"] = context.Notification.To.ConfirmationToken
                    });

                    var body = string.Format(email.Message, context.Notification.To.DisplayName, callbackUrl);

                    var message = new MailMessage()
                    {
                        Subject = email.Subject,
                        Body = WebUtility.HtmlDecode(body),
                        IsBodyHtml = true
                    };

                    message.To.Add(context.Notification.To.Email);

                    // send email
                    await emailManager.SaveAsync(message);

                }


                //// Services we need
                ////var backgroundTaskManager = context.ServiceProvider.GetRequiredService<IBackgroundTaskManager>();
                //var cacheManager = context.ServiceProvider.GetRequiredService<ICacheManager>();
                //var dbHelper = context.ServiceProvider.GetRequiredService<IDbHelper>();

                //const string sql = @"
                //    DECLARE @dirty bit = 0;
                //    DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                //    DECLARE @badgeName nvarchar(255) = '{name}';
                //    DECLARE @threshold int = {threshold};                  
                //    DECLARE @userId int;
                //    DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 u.Id FROM {prefix}_Users AS u
                //    WHERE (u.EmailConfirmed = 1)
                //    AND NOT EXISTS (
		              //       SELECT Id FROM {prefix}_UserBadges ub 
		              //       WHERE ub.UserId = u.Id AND ub.BadgeName = @badgeName
	               //     )
                //    ORDER BY u.Id DESC;
                    
                //    OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId;                    
                //    WHILE @@FETCH_STATUS = 0
                //    BEGIN
	               //     EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date;
                //        SET @dirty = 1;
	               //     FETCH NEXT FROM MSGCURSOR INTO @userId;	                    
                //    END;
                //    CLOSE MSGCURSOR;
                //    DEALLOCATE MSGCURSOR;
                //    SELECT @dirty;";

                //// Replacements for SQL script
                //var replacements = new Dictionary<string, string>()
                //{
                //    ["{name}"] = context.Badge.Name,
                //    ["{threshold}"] = context.Badge.Threshold.ToString()
                //};

                //// Start task to execute awarder SQL with replacements every X seconds
                //backgroundTaskManager.Start(async (sender, args) =>
                //{
                //    var dirty = await dbHelper.ExecuteScalarAsync<bool>(sql, replacements);
                //    if (dirty)
                //    {
                //        cacheManager.CancelTokens(typeof(UserBadgeStore));
                //    }

                //}, 240 * 1000);

            };

        }


    }


}
