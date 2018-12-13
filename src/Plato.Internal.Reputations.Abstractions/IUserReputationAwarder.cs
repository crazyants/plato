using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Reputations.Abstractions
{
    public interface IUserReputationAwarder
    {
        Task<UserReputation> AwardAsync(IReputation reputation, IUser user);
    }

}
