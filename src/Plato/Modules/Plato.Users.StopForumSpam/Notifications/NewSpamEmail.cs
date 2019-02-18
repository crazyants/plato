using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Users.StopForumSpam.Notifications
{
    public class NewSpamEmail : INotificationProvider<Badge>
    {


        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;

        public NewSpamEmail(
            IContextFacade contextFacade,
            ILocaleStore localeStore, 
            IEmailManager emailManager,
            ICapturedRouterUrlHelper capturedRouterUrlHelper)
        {
            _contextFacade = contextFacade;
            _localeStore = localeStore;
            _emailManager = emailManager;
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
        }

        public Task<ICommandResult<Badge>> SendAsync(INotificationContext<Badge> context)
        {
            throw new NotImplementedException();
        }

    }
}
