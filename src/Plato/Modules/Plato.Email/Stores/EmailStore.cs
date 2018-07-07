using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Email.Models;
using Plato.Email.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;

namespace Plato.Email.Stores
{

    public class EmailStore : IEmailStore<EmailMessage>
    {

        private readonly IEmailRepository<EmailMessage> _emailRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<EmailStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public EmailStore(IEmailRepository<EmailMessage> emailRepository, ICacheManager cacheManager, ILogger<EmailStore> logger, IDbQueryConfiguration dbQuery)
        {
            _emailRepository = emailRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<EmailMessage> CreateAsync(EmailMessage model)
        {
            return await _emailRepository.InsertUpdateAsync(model);
        }

        public async Task<EmailMessage> UpdateAsync(EmailMessage model)
        {
            return await _emailRepository.InsertUpdateAsync(model);
        }

        public async Task<bool> DeleteAsync(EmailMessage model)
        {

            var success = await _emailRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted email '{0}' with id {1}",
                        model.Subject, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;

        }

        public async Task<EmailMessage> GetByIdAsync(int id)
        {
            return await _emailRepository.SelectByIdAsync(id);
        }

        public IQuery<EmailMessage> QueryAsync()
        {
            var query = new EmailQuery(this);
            return _dbQuery.ConfigureQuery<EmailMessage>(query); ;
        }

        public async Task<IPagedResults<EmailMessage>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entities for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _emailRepository.SelectAsync(args);
              
            });
        }

        #endregion


    }
}
