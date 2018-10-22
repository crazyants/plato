using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;
using Plato.Reactions.Repositories;

namespace Plato.Reactions.Stores
{
    
    public class ReactionStore : IReactionStore<EntityReacttion>
    {

        private readonly IEntityReactionRepository<EntityReacttion> _entityReactionRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<ReactionStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public ReactionStore(
            IEntityReactionRepository<EntityReacttion> entityReactionRepository,
            ICacheManager cacheManager,
            ILogger<ReactionStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _entityReactionRepository = entityReactionRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<EntityReacttion> CreateAsync(EntityReacttion model)
        {
            var result = await _entityReactionRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<EntityReacttion> UpdateAsync(EntityReacttion model)
        {
            var result = await _entityReactionRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityReacttion model)
        {
            var success = await _entityReactionRepository.DeleteAsync(model.Id);
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

        public async Task<EntityReacttion> GetByIdAsync(int id)
        {
            return await _entityReactionRepository.SelectByIdAsync(id);
        }

        public IQuery<EntityReacttion> QueryAsync()
        {
            var query = new ReactionQuery(this);
            return _dbQuery.ConfigureQuery<EntityReacttion>(query); ;
        }

        public async Task<IPagedResults<EntityReacttion>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting reactions for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _entityReactionRepository.SelectAsync(args);

            });
        }

        #endregion

    }
}
