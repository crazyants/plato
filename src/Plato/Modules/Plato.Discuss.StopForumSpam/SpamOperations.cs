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
            Message = "Sorry but we've identified your details have been used by known spammers. You cannot post at this time. Please try updating your email address or username. If the problem persists please contact us and request we verify your account."
        };

        public static readonly SpamOperation Reply = new SpamOperation(
            "Replies",
            "Customize what will happen when replies are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            CustomMessage = true,
            Message = "Sorry but we've identified your details have been used by known spammers. You cannot post at this time. Please try updating your email address or username. If the problem persists please contact us and request we verify your account."
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
