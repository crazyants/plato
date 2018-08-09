namespace Plato.Internal.Models.Users
{
    
    public class SimpleUser : ISimpleUser
    {

        public int Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Alias { get; set; }

    }
}
