using Microsoft.AspNetCore.Identity;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Security.Abstractions
{
    public interface IDummyClaimsPrincipalFactory<TUser> : IUserClaimsPrincipalFactory<TUser> where TUser : class, IUser
    {
    }

}
