namespace Plato.Internal.Models.Users
{
    public class UserLogin : User
    {
        // UserLogin is simply a marker class so we can use
        // a separate view provider for the front-end user login page
        // This class should not contain any code

        public bool RememberMe { get; set; }

    }

}
