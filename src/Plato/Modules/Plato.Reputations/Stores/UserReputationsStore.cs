using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Reputations.Models;
using Plato.Reputations.Repositories;
using Plato.Reputations.Services;

namespace Plato.Reputations.Stores
{

    public class UserReputationsStore : IUserReputationsStore<UserReputation>
    {

        private readonly IUserReputationsRepository<UserReputation> _userReputationsRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<UserReputationsStore> _logger;
        private readonly IReputationsManager<Reputation> _badgesManager;

        public UserReputationsStore(
            IUserReputationsRepository<UserReputation> userReputationsRepository,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager,
            ILogger<UserReputationsStore> logger,
            IReputationsManager<Reputation> badgesManager)
        {
            _userReputationsRepository = userReputationsRepository;
            _dbQuery = dbQuery;
            _cacheManager = cacheManager;
            _logger = logger;
            _badgesManager = badgesManager;
        }

        public async Task<UserReputation> CreateAsync(UserReputation model)
        {

            var newUserReputation = await _userReputationsRepository.InsertUpdateAsync(model);
            if (newUserReputation != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Added new user reputation with id {1}",
                        newUserReputation.Id);
                }

                _cacheManager.CancelTokens(this.GetType());

            }

            return newUserReputation;

        }

        public async Task<UserReputation> UpdateAsync(UserReputation model)
        {

            var updatedUserReputation = await _userReputationsRepository.InsertUpdateAsync(model);
            if (updatedUserReputation != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Updated existing user reputation with id {1}",
                        updatedUserReputation.Id);
                }

                _cacheManager.CancelTokens(this.GetType());
            }

            return updatedUserReputation;

        }

        public async Task<bool> DeleteAsync(UserReputation model)
        {

            var success = await _userReputationsRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted user reputation '{0}' with id {1}",
                        model.Name, model.Id);
                }

                _cacheManager.CancelTokens(this.GetType());

            }

            return success;

        }

        public async Task<UserReputation> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userReputationsRepository.SelectByIdAsync(id));
        }

        public IQuery<UserReputation> QueryAsync()
        {
            var query = new UserReputationQuery(this);
            return _dbQuery.ConfigureQuery<UserReputation>(query); ;
        }

        public async Task<IPagedResults<UserReputation>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting user badges for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _userReputationsRepository.SelectAsync(args);

            });
        }

        public async Task<IEnumerable<IReputation>> GetUserReputationsAsync(int userId)
        {
            var reputations = _badgesManager.GetReputations();
            if (reputations == null)
            {
                return null;
            }

            var reputationList = reputations.ToList();
            if (reputationList.Count == 0)
            {
                return null;
            }

            var userReputations = await QueryAsync()
                .Select<UserReputationsQueryParams>(q =>
                {
                    q.UserId.Equals(userId);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();

            var output = new List<Reputation>();
            if (userReputations != null)
            {
                foreach (var userReputation in userReputations.Data)
                {
                    var reputation = reputationList.FirstOrDefault(b => b.Name.Equals(userReputation.Name, StringComparison.OrdinalIgnoreCase));
                    if (reputation != null)
                    {
                        reputation.AwardedDate = userReputation.CreatedDate;
                        output.Add(reputation);
                    }
                }
            }

            return output;

        }

    }


}
