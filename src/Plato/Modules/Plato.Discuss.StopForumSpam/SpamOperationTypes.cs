using System.Collections.Generic;
using Plato.StopForumSpam.Models;

namespace Plato.Discuss.StopForumSpam
{

    public class SpamOperationTypes : ISpamOperationTypeProvider<Plato.StopForumSpam.Models.SpamOperationType>
    {

        public static readonly Plato.StopForumSpam.Models.SpamOperationType Topic = new Plato.StopForumSpam.Models.SpamOperationType(
            "Topics",
            "Customize what will happen when topics are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin =  true,
            NotifyStaff = true,
            AllowAlter = true,
            Message = "Sorry but we've identified your details have been used by known spammers. Your account has been temporarily locked. You can login and continue to participate however your content won't appear until your account has been manually approved by an administrator or staff member."
        };

        public static readonly Plato.StopForumSpam.Models.SpamOperationType Reply = new Plato.StopForumSpam.Models.SpamOperationType(
            "Replies",
            "Customize what will happen when replies are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            AllowAlter = true,
            Message = "Sorry but we've identified your details have been used by known spammers. Your account has been temporarily locked. You will receive an email notification once an administrator or staff member has manually reviewed and white listed your account."
        };

        public IEnumerable<Plato.StopForumSpam.Models.SpamOperationType> GetSpamOperationTypes()
        {
            return new[]
            {
                Topic,
                Reply
            };
        }

    }

}
