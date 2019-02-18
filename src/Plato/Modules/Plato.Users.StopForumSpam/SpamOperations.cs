using System.Collections.Generic;
using Plato.StopForumSpam.Services;
using Plato.StopForumSpam.Models;

namespace Plato.Users.StopForumSpam
{

    public class SpamOperations : ISpamOperationProvider<SpamOperation>
    {

        public static readonly Plato.StopForumSpam.Models.SpamOperation Registration = new Plato.StopForumSpam.Models.SpamOperation(
            "Registration",
            "Customize what will happen when user registrations are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin =  true,
            NotifyStaff = true,
            CustomMessage = true,
            Message = "Sorry but we've identified your details have been used by known spammers. You cannot create an account with those details. Please try a different username or email address. If this error continues it could be your IP address has been used by known spammers. If your a genuine user please contact us for assistance."
        };

        public static readonly Plato.StopForumSpam.Models.SpamOperation Login = new Plato.StopForumSpam.Models.SpamOperation(
            "Login",
            "Customize what will happen when user logins are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            CustomMessage = false
        };

        public IEnumerable<SpamOperation> GetSpamOperations()
        {
            return new[]
            {
                Registration,
                Login
            };
        }

    }

}
