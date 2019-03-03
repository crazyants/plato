namespace Plato.Internal.Models.Users
{
    
    public class SimpleUser : ISimpleUser
    {

        public int Id { get; set; }

        public string UserName { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Alias { get; set; }

        public string PhotoUrl { get; set; }

        public string PhotoColor { get; set; }

        public string Signature { get; set; }

        public string SignatureHtml { get; set; }

        public UserAvatar Avatar => new UserAvatar(this);
        
        public SimpleUser()
        {
        
        }

        public SimpleUser(IUser user) : this()
        {
            Id = user.Id;
            UserName = user.UserName;
            DisplayName = user.DisplayName;
            Alias = user.Alias;
            PhotoUrl = user.PhotoUrl;
            PhotoColor = user.PhotoColor;
            Signature = user.Signature;
            SignatureHtml = user.SignatureHtml;
        }

    }


}
