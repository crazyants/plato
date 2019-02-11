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
        };

        public static readonly SpamOperation Login = new SpamOperation(
            "Login",
            "Customize what will happen when user logins are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            AllowAlter = true,
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
