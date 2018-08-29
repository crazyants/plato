using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Moderation.Models;
using Plato.Moderation.Repositories;

namespace Plato.Moderation.Stores
{

    public class ModeratorStore : IModeratorStore<Moderator>
    {

        private readonly IModeratorRepository<Moderator> _moderatorRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<ModeratorStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public ModeratorStore(
            IModeratorRepository<Moderator> moderatorRepository,
            ICacheManager cacheManager,
            ILogger<ModeratorStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _moderatorRepository = moderatorRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<Moderator> CreateAsync(Moderator model)
        {
            var result = await _moderatorRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;

        }

        public async Task<Moderator> UpdateAsync(Moderator model)
        {
            var result = await _moderatorRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;

        }

        public async Task<bool> DeleteAsync(Moderator model)
        {

            var success = await _moderatorRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted moderator for user '{0}' and category '{1}' with id {2}",
                        model.UserId, model.CategoryId, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;

        }

        public async Task<Moderator> GetByIdAsync(int id)
        {
            return await _moderatorRepository.SelectByIdAsync(id);
        }

        public IQuery<Moderator> QueryAsync()
        {
            var query = new ModeratorQuery(this);
            return _dbQuery.ConfigureQuery<Moderator>(query); ;
        }

        public async Task<IPagedResults<Moderator>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting emails for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _moderatorRepository.SelectAsync(args);

            });
        }

        #endregion

    }


}
