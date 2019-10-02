using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.History
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewEntityHistory =
            new Permission("ViewQuestionHistory", "View question edit history");

        public static readonly Permission viewReplyHistory =
            new Permission("ViewAnswertHistory", "View answer edit history");
        
        public static readonly Permission RevertEntityHistory =
            new Permission("RevertQuestionHistory", "Revert questions to previous versions");

        public static readonly Permission RevertReplyHistory =
            new Permission("RevertAnswerHistory", "Revert answers to previous versions");
        
        public static readonly Permission DeleteEntityHistory =
            new Permission("DeleteQuestionHistory", "Delete question versions");

        public static readonly Permission DeleteReplyHistory =
            new Permission("DeleteAnswerHistory", "Delete answer versions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewEntityHistory,
                viewReplyHistory,
                RevertEntityHistory,
                RevertReplyHistory,
                DeleteEntityHistory,
                DeleteReplyHistory
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
                        ViewEntityHistory,
                        viewReplyHistory,
                        RevertEntityHistory,
                        RevertReplyHistory,
                        DeleteEntityHistory,
                        DeleteReplyHistory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                       ViewEntityHistory,
                        viewReplyHistory,
                        RevertEntityHistory,
                        RevertReplyHistory
                    }
                }
            };
        }

    }

}
