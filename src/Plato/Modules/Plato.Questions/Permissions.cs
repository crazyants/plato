using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostQuestions =
            new Permission("PostQuestions", "Post questions");

        public static readonly Permission PostAnswers =
            new Permission("PostAnswers", "Post answers");

        public static readonly Permission EditOwnQuestions =
            new Permission("EditOwnQuestions", "Edit own questions");

        public static readonly Permission EditAnyQuestion =
            new Permission("EditAnyQuestion", "Edit any questions");
        
        public static readonly Permission EditOwnAnswers =
            new Permission("EditOwnAnswers", "Edit own answers");

        public static readonly Permission EditAnyAnswer =
            new Permission("EditAnyAnswer", "Edit any answer");
        
        public static readonly Permission DeleteOwnQuestions = 
            new Permission("DeleteOwnQuestions", "Delete own questions");

        public static readonly Permission RestoreOwnQuestions =
            new Permission("RestoreOwnQuestions", "Restore own questions");
        
        public static readonly Permission DeleteAnyQuestion =
            new Permission("DeleteAnyQuestion", "Delete any question");

        public static readonly Permission RestoreAnyQuestion =
            new Permission("RestoreAnyQuestion", "Restore any question");
        
        public static readonly Permission DeleteOwnAnswers =
            new Permission("DeleteOwnAnswers", "Delete own answers");

        public static readonly Permission RestoreOwnAnswers =
            new Permission("RestoreOwnAnswers", "Restore own answers");
        
        public static readonly Permission DeleteAnyAnswer =
            new Permission("DeleteAnyAnswer", "Delete any answer");

        public static readonly Permission RestoreAnyAnswer =
            new Permission("RestoreAnyAnswer", "Restore any answer");

        public static readonly Permission ReportQuestions =
            new Permission("ReportQuestions", "Report questions");

        public static readonly Permission ReportAnswers =
            new Permission("ReportAnswers", "Report answers");

        public static readonly Permission ViewPrivateQuestions =
            new Permission("ViewPrivateQuestions", "View private questions");

        public static readonly Permission ViewPrivateAnswers =
            new Permission("ViewPrivateAnswers", "View private answers");

        public static readonly Permission ViewSpamQuestions =
            new Permission("ViewSpamQuestions", "View questions flagged as SPAM");

        public static readonly Permission ViewSpamAnswers =
            new Permission("ViewSpamAnswers", "View answers flagged as SPAM");
        
        public static readonly Permission ViewDeletedQuestions =
            new Permission("ViewDeletedQuestions", "View deleted questions");

        public static readonly Permission ViewDeletedAnswers =
            new Permission("ViewDeletedComments", "View deleted answers");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostQuestions,
                PostAnswers,
                EditOwnQuestions,
                EditAnyQuestion,
                EditOwnAnswers,
                EditAnyAnswer,
                DeleteOwnQuestions,
                RestoreOwnQuestions,
                DeleteAnyQuestion,
                RestoreAnyQuestion,
                DeleteOwnAnswers,
                RestoreOwnAnswers,
                DeleteAnyAnswer,
                RestoreAnyAnswer,
                ReportQuestions,
                ReportAnswers,
                ViewPrivateQuestions,
                ViewPrivateAnswers,
                ViewSpamQuestions,
                ViewSpamAnswers,
                ViewDeletedQuestions,
                ViewDeletedAnswers
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
                        PostQuestions,
                        PostAnswers,
                        EditOwnQuestions,
                        EditAnyQuestion,
                        EditOwnAnswers,
                        EditAnyAnswer,
                        DeleteOwnQuestions,
                        RestoreOwnQuestions,
                        DeleteAnyQuestion,
                        RestoreAnyQuestion,
                        DeleteOwnAnswers,
                        RestoreOwnAnswers,
                        DeleteAnyAnswer,
                        RestoreAnyAnswer,
                        ReportQuestions,
                        ReportAnswers,
                        ViewPrivateQuestions,
                        ViewPrivateAnswers,
                        ViewSpamQuestions,
                        ViewSpamAnswers,
                        ViewDeletedQuestions,
                        ViewDeletedAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostQuestions,
                        PostAnswers,
                        EditOwnQuestions,
                        EditOwnAnswers,
                        DeleteOwnAnswers,
                        ReportQuestions,
                        ReportAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostQuestions,
                        PostAnswers,
                        EditOwnQuestions,
                        EditOwnAnswers,
                        DeleteOwnQuestions,
                        RestoreOwnQuestions,
                        DeleteOwnAnswers,
                        RestoreOwnAnswers,
                        ReportQuestions,
                        ReportAnswers,
                        ViewPrivateQuestions,
                        ViewPrivateAnswers,
                        ViewSpamQuestions,
                        ViewSpamAnswers,
                        ViewDeletedQuestions,
                        ViewDeletedAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportQuestions,
                        ReportAnswers
                    }
                }
            };
        }

    }

}
