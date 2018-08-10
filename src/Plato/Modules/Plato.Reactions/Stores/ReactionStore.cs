using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;
using Plato.Reactions.Repositories;

namespace Plato.Reactions.Stores
{
    
    public class ReactionStore : IReactionStore<Reaction>
    {

        private readonly IReactionRepository<Reaction> _reactionRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<ReactionStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public ReactionStore(
            IReactionRepository<Reaction> reactionRepository,
            ICacheManager cacheManager,
            ILogger<ReactionStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _reactionRepository = reactionRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<Reaction> CreateAsync(Reaction model)
        {
            var result = await _reactionRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<Reaction> UpdateAsync(Reaction model)
        {
            var result = await _reactionRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Reaction model)
        {
            var success = await _reactionRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted reaction '{0}' with id {1}",
                        model.Name, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<Reaction> GetByIdAsync(int id)
        {
            return await _reactionRepository.SelectByIdAsync(id);
        }

        public IQuery<Reaction> QueryAsync()
        {
            var query = new ReactionQuery(this);
            return _dbQuery.ConfigureQuery<Reaction>(query); ;
        }

        public async Task<IPagedResults<Reaction>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting reactions for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _reactionRepository.SelectAsync(args);

            });
        }

        #endregion

    }
}
