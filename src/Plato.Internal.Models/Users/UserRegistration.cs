namespace Plato.Internal.Models.Users
{
    public class UserRegistration : User
    {
        // UserProfile is simply a marker class so we can use
        // a separate view provider for the front-end user registration page
        // This class should not contain any code

        public bool IsNewUser { get; set; }

        public string ConfirmPassword { get; set; }

    }

}
