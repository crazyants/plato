namespace Plato.Internal.Models.Users
{
    public interface ISimpleUser
    {

        int Id { get; set; }
        
        string UserName { get; set; }

        string DisplayName { get; set; }
        
        string Alias { get; set; }

        string PhotoUrl { get; set; }

        string PhotoColor { get; set; }

        string Signature { get; set; }

        string SignatureHtml { get; set; }
        
        UserAvatar Avatar { get; }

        UserUrls Urls { get; }
    }

}
