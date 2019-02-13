using System.Collections.Generic;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.Discuss.StopForumSpam
{

    public class SpamOperations : ISpamOperationProvider<SpamOperation>
    {

        public static readonly SpamOperation Topic = new SpamOperation(
            "Topics",
            "Customize what will happen when topics are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            CustomMessage = true,
            Message = "Sorry but we've identified your details have been used by known spammers. Your account has been temporarily locked. You can login and continue to participate however your content won't appear until your account has been manually approved by an administrator or staff member."
        };

        public static readonly SpamOperation Reply = new SpamOperation(
            "Replies",
            "Customize what will happen when replies are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            CustomMessage = true,
            Message = "Sorry but we've identified your details have been used by known spammers. Your account has been temporarily locked. You will receive an email notification once an administrator or staff member has manually reviewed and white listed your account."
        };

        public IEnumerable<SpamOperation> GetSpamOperations()
        {
            return new[]
            {
                Topic,
                Reply
            };
        }

    }

}
