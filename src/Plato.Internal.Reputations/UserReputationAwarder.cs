using System;
using System.Threading.Tasks;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Models.Reputations;

namespace Plato.Internal.Reputations
{
    public class UserReputationAwarder : IUserReputationAwarder
    {

        private readonly IUserReputationManager<UserReputation> _userReputationManager;

        public UserReputationAwarder(
            IUserReputationManager<UserReputation> userReputationManager)
        {
            _userReputationManager = userReputationManager;
        }

        public async Task<UserReputation> AwardAsync(IReputation reputation, int userId)
        {

            if (reputation == null)
            {
                throw new ArgumentNullException(nameof(reputation));
            }

            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            var userReputation = new UserReputation()
            {
                Name = reputation.Name,
                Points = reputation.Points,
                CreatedUserId = userId
            };

            var result = await _userReputationManager.CreateAsync(userReputation);
            if (result.Succeeded)
            {
                return result.Response;
            }

            return null;

        }
    }
}
