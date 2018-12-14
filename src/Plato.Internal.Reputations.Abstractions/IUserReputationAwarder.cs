using System.Threading.Tasks;

namespace Plato.Internal.Reputations.Abstractions
{
    public interface IUserReputationAwarder
    {
        Task<UserReputation> AwardAsync(IReputation reputation, int userId);
    }

}
