namespace Plato.Internal.Security.Abstractions
{
    public class StandardPermissions
    {

        public const string CategoryName = "Plato";

        public static readonly Permission AdminAccess = 
            new Permission("AdminAccess", "Can access administrator dashboard", CategoryName);

    }
}
