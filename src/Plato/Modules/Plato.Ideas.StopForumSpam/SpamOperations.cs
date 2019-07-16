using System.Collections.Generic;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.Ideas.StopForumSpam
{

    public class SpamOperations : ISpamOperationProvider<SpamOperation>
    {

        public static readonly SpamOperation Idea = new SpamOperation(
            "Ideas",
            "Ideas",
            "Customize what will happen when ideas are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            CustomMessage = false,
            Message = "Sorry but we've identified your details have been used by known spammers. You cannot post at this time. Please try updating your email address or username. If the problem persists please contact us and request we verify your account."
        };

        public static readonly SpamOperation Comment = new SpamOperation(
            "IdeaComments",
            "Idea Comments",
            "Customize what will happen when idea comments are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            CustomMessage = false,
            Message = "Sorry but we've identified your details have been used by known spammers. You cannot post at this time. Please try updating your email address or username. If the problem persists please contact us and request we verify your account."
        };

        public IEnumerable<SpamOperation> GetSpamOperations()
        {
            return new[]
            {
                Idea,
                Comment
            };
        }

    }

}
