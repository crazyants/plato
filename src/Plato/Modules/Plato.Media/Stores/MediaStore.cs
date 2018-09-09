using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Media.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Media.Stores
{

    public class MediaStore : IMediaStore<Models.Media>
    {

        private readonly IMediaRepository<Models.Media> _emailRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<MediaStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public MediaStore(
            IMediaRepository<Models.Media> emailRepository, 
            ICacheManager cacheManager,
            ILogger<MediaStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _emailRepository = emailRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<Models.Media> CreateAsync(Models.Media model)
        {
            var result = await _emailRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;

        }

        public async Task<Models.Media> UpdateAsync(Models.Media model)
        {
            var result = await _emailRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;

        }

        public async Task<bool> DeleteAsync(Models.Media model)
        {

            var success = await _emailRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted email '{0}' with id {1}",
                        model.Name, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;

        }

        public async Task<Models.Media> GetByIdAsync(int id)
        {
            return await _emailRepository.SelectByIdAsync(id);
        }

        public IQuery<Models.Media> QueryAsync()
        {
            var query = new MediaQuery(this);
            return _dbQuery.ConfigureQuery<Models.Media>(query); ;
        }

        public async Task<IPagedResults<Models.Media>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting media for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _emailRepository.SelectAsync(args);
              
            });
        }

        #endregion
        
    }
}
