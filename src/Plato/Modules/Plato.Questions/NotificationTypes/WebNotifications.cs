using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification QuestionReport =
            new WebNotification("QuestionReportWeb", "Reported Articles",
                "Show me a web notification each time a question is reported.");

        public static readonly WebNotification AnswerReport =
            new WebNotification("AnswerReportWeb", "Reported Comments",
                "Show me a web notification each time a questions answer is reported.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        QuestionReport,
                        AnswerReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        QuestionReport,
                        AnswerReport
                    }
                }

            };
        }

        public IEnumerable<DefaultNotificationTypes> GetDefaultNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        QuestionReport,
                        AnswerReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        QuestionReport,
                        AnswerReport
                    }
                }
            };
        }

    }

}
