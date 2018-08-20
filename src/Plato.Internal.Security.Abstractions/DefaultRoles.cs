namespace Plato.Internal.Security.Abstractions
{
    public static class DefaultRoles
    {

        public const string Anonymous = "Anonymous";
        public const string Member = "Member";
        public const string Employee = "Employee";
        public const string Administrator = "Administrator";
        
        public static string[] ToList() => new[]
        {
            Anonymous,
            Member,
            Employee,
            Administrator
        };

    }
}
