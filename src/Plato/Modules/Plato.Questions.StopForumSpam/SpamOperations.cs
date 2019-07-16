using System.Collections.Generic;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.Questions.StopForumSpam
{

    public class SpamOperations : ISpamOperationProvider<SpamOperation>
    {

        public static readonly SpamOperation Question = new SpamOperation(
            "Questions",
            "Questions",
            "Customize what will happen when questions are detected as SPAM.")
        {
            FlagAsSpam = true,
            NotifyAdmin = true,
            NotifyStaff = true,
            CustomMessage = false,
            Message = "Sorry but we've identified your details have been used by known spammers. You cannot post at this time. Please try updating your email address or username. If the problem persists please contact us and request we verify your account."
        };

        public static readonly SpamOperation Answer = new SpamOperation(
            "QuestionAnswers",
            "Question Answers",
            "Customize what will happen when question answers are detected as SPAM.")
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
                Question,
                Answer
            };
        }

    }

}
