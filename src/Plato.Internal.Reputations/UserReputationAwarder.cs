using System.Threading.Tasks;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Internal.Reputations
{
    public class UserReputationAwarder : IUserReputationAwarder
    {

        public Task<UserReputation> AwardAsync(IReputation reputation, int userId)
        {
            // Dummy implementation. 
            // The implementation is replaced
            // when Plato.Reputations is enabled
            return Task.FromResult(default(UserReputation));
        }
    }

}
