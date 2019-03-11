using System.Threading.Tasks;
using Plato.Internal.Models.Reputations;

namespace Plato.Internal.Reputations.Abstractions
{
    public interface IUserReputationAwarder
    {
        Task<UserReputation> AwardAsync(IReputation reputation, int userId, string description);

        Task<UserReputation> RevokeAsync(IReputation reputation, int userId, string description);

    }

}
