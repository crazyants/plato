using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Reactions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ReactToQuestions =
            new Permission("ReactToQuestions", "React to questions");

        public static readonly Permission ReactToAnswers =
            new Permission("ReactToAnswers", "React to question answers");

        public static readonly Permission ViewQuestionReactions =
            new Permission("ViewQuestionReactions", "View question reactions");

        public static readonly Permission ViewAnswerReactions =
            new Permission("ViewAnswerReactions", "View question answer reactions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ReactToQuestions,
                ReactToAnswers,
                ViewQuestionReactions,
                ViewAnswerReactions
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        ReactToQuestions,
                        ReactToAnswers,
                        ViewQuestionReactions,
                        ViewAnswerReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ReactToQuestions,
                        ReactToAnswers,
                        ViewQuestionReactions,
                        ViewAnswerReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ReactToQuestions,
                        ReactToAnswers,
                        ViewQuestionReactions,
                        ViewAnswerReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewQuestionReactions,
                        ViewAnswerReactions
                    }
                }
            };

        }

    }

}
