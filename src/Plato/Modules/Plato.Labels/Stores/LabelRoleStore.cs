using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Labels.Models;
using Plato.Labels.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;

namespace Plato.Labels.Stores
{

    public class LabelRoleStore : ILabelRoleStore<LabelRole>
    {
        private const string ByIdKey = "ById";
        private const string ByLabelIdKey = "ByLabelId";

        private readonly ILabelRoleRepository<LabelRole> _LabelRoleRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<LabelRoleStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public LabelRoleStore(
            ILabelRoleRepository<LabelRole> LabelRoleRepository,
            ICacheManager cacheManager,
            ILogger<LabelRoleStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _LabelRoleRepository = LabelRoleRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<LabelRole> CreateAsync(LabelRole model)
        {
            var result = await _LabelRoleRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByIdKey, model.Id);
                _cacheManager.CancelTokens(this.GetType(), ByLabelIdKey, model.LabelId);
            }

            return result;
        }

        public async Task<LabelRole> UpdateAsync(LabelRole model)
        {
            var result = await _LabelRoleRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByIdKey, model.Id);
                _cacheManager.CancelTokens(this.GetType(), ByLabelIdKey, model.LabelId);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(LabelRole model)
        {

            var success = await _LabelRoleRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted Label role for Label '{0}' with id {1}",
                        model.LabelId, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByIdKey, model.Id);
                _cacheManager.CancelTokens(this.GetType(), ByLabelIdKey, model.LabelId);
            }

            return success;

        }

        public async Task<LabelRole> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByIdKey, id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _LabelRoleRepository.SelectByIdAsync(id));

        }

        public IQuery<LabelRole> QueryAsync()
        {
            var query = new LabelRoleQuery(this);
            return _dbQuery.ConfigureQuery<LabelRole>(query); ;
        }

        public async Task<IPagedResults<LabelRole>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entities for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _LabelRoleRepository.SelectAsync(args);

            });
        }

        #endregion
        
        public async Task<IEnumerable<LabelRole>> GetByLabelIdAsync(int LabelId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByLabelIdKey, LabelId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting roles for Label Id '{0}'",
                        LabelId);
                }

                return await _LabelRoleRepository.SelectByLabelIdAsync(LabelId);
           
            });

        }

        public async Task<bool> DeleteByLabelIdAsync(int LabelId)
        {
            
            var success = await _LabelRoleRepository.DeleteByLabelIdAsync(LabelId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all Label roles for Label '{0}'",
                        LabelId);
                }
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByLabelIdKey, LabelId);
            
            }

            return success;

        }

        public async Task<bool> DeleteByRoleIdAndLabelIdAsync(int roleId, int LabelId)
        {
            var success = await _LabelRoleRepository.DeleteByRoleIdAndLabelIdAsync(roleId, LabelId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted Label role for role '{0}' and Label '{1}'",
                      roleId, LabelId);
                }
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByLabelIdKey, LabelId);

            }

            return success;
        }
    }

}
