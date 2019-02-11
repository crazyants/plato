using System.Collections.Generic;
using Plato.StopForumSpam.Models;

namespace Plato.Users.StopForumSpam
{

    public class SpamOperations : ISpamOperationsProvider<SpamOperation>
    {

        public static readonly SpamOperation Registration = new SpamOperation(
            "Registration",
            "Customize what will happen when user registrations are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin =  true,
            NotifyStaff = true,
            AllowAlter = true,
            Message = "Sorry but we've identified your details have been used by known spammers. Your account has been temporarily locked. You can login and continue to participate however your content won't appear until your account has been manually approved by an administrator or staff member."
        };

        public static readonly SpamOperation Login = new SpamOperation(
            "Login",
            "Customize what will happen when user logins are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            AllowAlter = true,
            Message = "Sorry but we've identified your details have been used by known spammers. Your account has been temporarily locked. You will receive an email notification once an administrator or staff member has manually reviewed and white listed your account."
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
