using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Repositories;

namespace Plato.Labels.Stores
{

    public class LabelRoleStore : ILabelRoleStore<LabelRole>
    {
        private const string ByIdKey = "ById";
        private const string ByLabelIdKey = "ByLabelId";

        private readonly ILabelRoleRepository<LabelRole> _labelRoleRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<LabelRoleStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public LabelRoleStore(
            ILabelRoleRepository<LabelRole> labelRoleRepository,
            ICacheManager cacheManager,
            ILogger<LabelRoleStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _labelRoleRepository = labelRoleRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<LabelRole> CreateAsync(LabelRole model)
        {
            var result = await _labelRoleRepository.InsertUpdateAsync(model);
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
            var result = await _labelRoleRepository.InsertUpdateAsync(model);
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

            var success = await _labelRoleRepository.DeleteAsync(model.Id);
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
                async (cacheEntry) => await _labelRoleRepository.SelectByIdAsync(id));

        }

        public IQuery<LabelRole> QueryAsync()
        {
            var query = new LabelRoleQuery(this);
            return _dbQuery.ConfigureQuery<LabelRole>(query); ;
        }

        public async Task<IPagedResults<LabelRole>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entities for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _labelRoleRepository.SelectAsync(dbParams);

            });
        }

        #endregion
        
        public async Task<IEnumerable<LabelRole>> GetByLabelIdAsync(int labelId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByLabelIdKey, labelId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting roles for Label Id '{0}'",
                        labelId);
                }

                return await _labelRoleRepository.SelectByLabelIdAsync(labelId);
           
            });

        }

        public async Task<bool> DeleteByLabelIdAsync(int labelId)
        {
            
            var success = await _labelRoleRepository.DeleteByLabelIdAsync(labelId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all Label roles for Label '{0}'",
                        labelId);
                }
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByLabelIdKey, labelId);
            
            }

            return success;

        }

        public async Task<bool> DeleteByRoleIdAndLabelIdAsync(int roleId, int labelId)
        {

            var success = await _labelRoleRepository.DeleteByRoleIdAndLabelIdAsync(roleId, labelId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted Label role for role '{0}' and Label '{1}'",
                      roleId, labelId);
                }
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByLabelIdKey, labelId);

            }

            return success;
        }

    }

}
