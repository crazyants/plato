namespace Plato.Internal.Models.Users
{
    
    public class SimpleUser : ISimpleUser
    {

        public int Id { get; set; }

        public string UserName { get; set; }
        
        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Alias { get; set; }

        public SimpleUser()
        {

        }

        public SimpleUser(IUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            DisplayName = user.DisplayName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Alias = user.Alias;
        }

    }
}
