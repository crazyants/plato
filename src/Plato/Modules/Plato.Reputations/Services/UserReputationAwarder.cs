using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Reputations.Services
{
    public class UserReputationAwarder : IUserReputationAwarder
    {

        private readonly IUserReputationManager<UserReputation> _userReputationManager;

        public UserReputationAwarder(
            IUserReputationManager<UserReputation> userReputationManager)
        {
            _userReputationManager = userReputationManager;
        }

        public async Task<UserReputation> AwardAsync(IReputation reputation, IUser user)
        {

            if (reputation == null)
            {
                throw new ArgumentNullException(nameof(reputation));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(user.Id));
            }

            var userReputation = new UserReputation()
            {
                Name = reputation.Name,
                Points = reputation.Points,
                CreatedUserId = user.Id
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
