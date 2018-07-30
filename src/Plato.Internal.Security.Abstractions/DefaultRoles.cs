namespace Plato.Internal.Security.Abstractions
{
    public static class DefaultRoles
    {
        public const string Administrator = "Administrator";
        public const string Employee = "Employee";
        public const string Member = "Member";
        public const string Guest = "Guest";

        public static string[] ToList()
        {
            return new string[]
            {
                Administrator,
                Employee,
                Member,
                Guest
            };
        }

    }
}
