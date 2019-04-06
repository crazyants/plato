using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostDocs =
            new Permission("PostDocs", "Post documentation");

        public static readonly Permission PostDocComments =
            new Permission("PostDocComments", "Post comments to documentation");
        
        public static readonly Permission EditOwnDocs =
            new Permission("EditOwnDocs", "Edit own documentation");

        public static readonly Permission EditAnyDoc =
            new Permission("EditAnyDoc", "Edit any documentation");

        public static readonly Permission EditOwnDocComments =
            new Permission("EditOwnDocComments", "Edit own documentation comments");

        public static readonly Permission EditAnyDocComment =
            new Permission("EditAnyDocComment", "Edit any documentation comment");
        
        public static readonly Permission DeleteOwnDocs = 
            new Permission("DeleteOwnDocs", "Delete own documentation");

        public static readonly Permission RestoreOwnDocs =
            new Permission("RestoreOwnDocs", "Restore own documentation");
        
        public static readonly Permission DeleteAnyDoc =
            new Permission("DeleteAnyDoc", "Delete any documentation");

        public static readonly Permission RestoreAnyDoc =
            new Permission("RestoreAnyDoc", "Restore any documentation");
        
        public static readonly Permission DeleteOwnDocComments =
            new Permission("DeleteOwnDocComments", "Delete own documentation comments");

        public static readonly Permission RestoreOwnDocComments =
            new Permission("RestoreOwnDocComments", "Restore own documentation comments");
        
        public static readonly Permission DeleteAnyDocComment =
            new Permission("DeleteAnyDocComment", "Delete any documentation comment");

        public static readonly Permission RestoreAnyDocComment =
            new Permission("RestoreAnyDocComment", "Restore any documentation comment");

        public static readonly Permission ReportDocs =
            new Permission("ReportDocs", "Report documentation");

        public static readonly Permission ReportDocComments =
            new Permission("ReportDocComments", "Report documentation comments");

        public static readonly Permission ViewPrivateDocs =
            new Permission("ViewPrivateDocs", "View hidden documentation");

        public static readonly Permission ViewPrivateDocComments =
            new Permission("ViewPrivateDocComments", "View hidden documentation comments");

        public static readonly Permission ViewSpamDocs =
            new Permission("ViewSpamDocs", "View documentation flagged as SPAM");

        public static readonly Permission ViewSpamDocComments =
            new Permission("ViewSpamDocComments", "View documentation comments flagged as SPAM");
        
        public static readonly Permission ViewDeletedDocs =
            new Permission("ViewDeletedDocs", "View deleted documentation");

        public static readonly Permission ViewDeletedDocComments =
            new Permission("ViewDeletedDocComments", "View deleted documentation comments");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostDocs,
                PostDocComments,
                EditOwnDocs,
                EditAnyDoc,
                EditOwnDocComments,
                EditAnyDocComment,
                DeleteOwnDocs,
                RestoreOwnDocs,
                DeleteAnyDoc,
                RestoreAnyDoc,
                DeleteOwnDocComments,
                RestoreOwnDocComments,
                DeleteAnyDocComment,
                RestoreAnyDocComment,
                ReportDocs,
                ReportDocComments,
                ViewPrivateDocs,
                ViewPrivateDocComments,
                ViewSpamDocs,
                ViewSpamDocComments,
                ViewDeletedDocs,
                ViewDeletedDocComments
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
                        PostDocs,
                        PostDocComments,
                        EditOwnDocs,
                        EditAnyDoc,
                        EditOwnDocComments,
                        EditAnyDocComment,
                        DeleteOwnDocs,
                        RestoreOwnDocs,
                        DeleteAnyDoc,
                        RestoreAnyDoc,
                        DeleteOwnDocComments,
                        RestoreOwnDocComments,
                        DeleteAnyDocComment,
                        RestoreAnyDocComment,
                        ReportDocs,
                        ReportDocComments,
                        ViewPrivateDocs,
                        ViewPrivateDocComments,
                        ViewSpamDocs,
                        ViewSpamDocComments,
                        ViewDeletedDocs,
                        ViewDeletedDocComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostDocComments,
                        EditOwnDocComments,
                        DeleteOwnDocComments,
                        ReportDocComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostDocs,
                        PostDocComments,
                        EditOwnDocs,
                        EditOwnDocComments,
                        DeleteOwnDocs,
                        RestoreOwnDocs,
                        DeleteOwnDocComments,
                        RestoreOwnDocComments,
                        ReportDocs,
                        ReportDocComments,
                        ViewPrivateDocs,
                        ViewPrivateDocComments,
                        ViewSpamDocs,
                        ViewSpamDocComments,
                        ViewDeletedDocs,
                        ViewDeletedDocComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportDocs,
                        ReportDocComments
                    }
                }
            };
        }

    }

}
