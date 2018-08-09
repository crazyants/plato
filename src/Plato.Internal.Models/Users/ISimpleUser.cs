namespace Plato.Internal.Models.Users
{
    public interface ISimpleUser
    {

        int Id { get; set; }

        string Email { get; set; }

        string UserName { get; set; }

        string NormalizedUserName { get; set; }

        string DisplayName { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string Alias { get; set; }

    }


}
